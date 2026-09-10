using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AlBlockCarrier : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private AlMovement alMovement;

    [Header("Largar Bloco")]
    [SerializeField] private float dropDistance = 0.8f;
    [SerializeField] private float dropCheckRadius = 0.4f;
    [SerializeField] private LayerMask dropObstacleMask;

    [Header("Áudio")]
    [SerializeField] private AudioClip errorSound;

    private readonly HashSet<DadoBlock> _nearbyBlocks = new();
    private readonly HashSet<NumericSlot> _nearbySlots = new();
    private readonly HashSet<ConfirmationLever> _nearbyLevers = new();
    private DadoBlock _highlightedBlock;
    private NumericSlot _highlightedSlot;
    private ConfirmationLever _highlightedLever;
    private DadoBlock _heldBlock;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (alMovement == null)
        {
            alMovement = GetComponent<AlMovement>();
        }
    }

    private void Update()
    {
        if (_heldBlock == null)
        {
            UpdateHighlight();
        }
        else
        {
            TryRotateHeldBlock();
            UpdateSlotHighlight();
        }

        if (InputManager.Instance == null || !InputManager.Instance.AlInteractPressed)
        {
            return;
        }

        if (_heldBlock != null)
        {
            HandleInteractWhileHolding();
        }
        else if (_highlightedBlock != null)
        {
            PickupHighlightedBlock();
        }
        else
        {
            TryConfirmNearbyLever();
        }
    }

    private void TryConfirmNearbyLever()
    {
        ConfirmationLever lever = FindClosestLever();
        lever?.Confirm();
    }

    private ConfirmationLever FindClosestLever()
    {
        ConfirmationLever closest = null;
        float closestDistance = float.MaxValue;

        foreach (ConfirmationLever lever in _nearbyLevers)
        {
            float distance = Vector2.Distance(transform.position, lever.transform.position);

            if (distance < closestDistance)
            {
                closest = lever;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void HandleInteractWhileHolding()
    {
        NumericSlot targetSlot = FindClosestEmptySlot();

        if (targetSlot != null)
        {
            InsertHeldBlockIntoSlot(targetSlot);
        }
        else
        {
            TryDropHeldBlock();
        }
    }

    private NumericSlot FindClosestEmptySlot()
    {
        NumericSlot closest = null;
        float closestDistance = float.MaxValue;

        foreach (NumericSlot slot in _nearbySlots)
        {
            if (slot.IsFilled)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, slot.transform.position);

            if (distance < closestDistance)
            {
                closest = slot;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void UpdateSlotHighlight()
    {
        NumericSlot closest = FindClosestEmptySlot();

        if (closest == _highlightedSlot)
        {
            return;
        }

        _highlightedSlot?.SetHighlighted(false);
        _highlightedSlot = closest;
        _highlightedSlot?.SetHighlighted(true);
    }

    private void InsertHeldBlockIntoSlot(NumericSlot slot)
    {
        DadoBlock block = _heldBlock;
        bool inserted = slot.TryInsertBlock(block);

        if (inserted)
        {
            _heldBlock = null;
            slot.SetHighlighted(false);
            _highlightedSlot = null;

            // Garantia manual: reativar o collider e reposicionar o bloco
            // no mesmo frame (dentro de PlaceInSlot) pode não disparar um
            // novo OnTriggerEnter2D de forma confiável. Adicionamos aqui
            // diretamente, já que para inserir o Al precisa estar por
            // perto de qualquer forma.
            _nearbyBlocks.Add(block);
        }
        else
        {
            // Slot ficou ocupado entre a detecção e o clique (raro, mas
            // possível) — mantém o bloco na mão em vez de perdê-lo.
            PlayErrorSound();
        }
    }

    private void TryRotateHeldBlock()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        if (InputManager.Instance.AlRotateFaceLeftPressed)
        {
            _heldBlock.RotateFaceLeft();
        }
        else if (InputManager.Instance.AlRotateFaceRightPressed)
        {
            _heldBlock.RotateFaceRight();
        }
    }

    private void UpdateHighlight()
    {
        DadoBlock closestBlock = FindClosestBlock();

        if (closestBlock != null)
        {
            SetBlockHighlight(closestBlock);
            SetLeverHighlight(null); // bloco tem prioridade; nunca destaca os dois ao mesmo tempo
            return;
        }

        SetBlockHighlight(null);
        SetLeverHighlight(FindClosestLever());
    }

    private void SetBlockHighlight(DadoBlock block)
    {
        if (block == _highlightedBlock)
        {
            return;
        }

        _highlightedBlock?.SetHighlighted(false);
        _highlightedBlock = block;
        _highlightedBlock?.SetHighlighted(true);
    }

    private void SetLeverHighlight(ConfirmationLever lever)
    {
        if (lever == _highlightedLever)
        {
            return;
        }

        _highlightedLever?.SetHighlighted(false);
        _highlightedLever = lever;
        _highlightedLever?.SetHighlighted(true);
    }

    private DadoBlock FindClosestBlock()
    {
        DadoBlock closest = null;
        float closestDistance = float.MaxValue;

        foreach (DadoBlock block in _nearbyBlocks)
        {
            float distance = Vector2.Distance(transform.position, block.transform.position);

            if (distance < closestDistance)
            {
                closest = block;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void PickupHighlightedBlock()
    {
        DadoBlock block = _highlightedBlock;

        _nearbyBlocks.Remove(block);
        _highlightedBlock = null;

        _highlightedLever?.SetHighlighted(false);
        _highlightedLever = null;

        _heldBlock = block;
        _heldBlock.Pickup(holdPoint);
    }

    private void TryDropHeldBlock()
    {
        Vector3 dropPosition = transform.position + (Vector3)(alMovement.FacingDirection * dropDistance);

        bool isBlocked = Physics2D.OverlapCircle(dropPosition, dropCheckRadius, dropObstacleMask);

        if (isBlocked)
        {
            PlayErrorSound();
            return;
        }

        _heldBlock.Drop(dropPosition);
        _heldBlock = null;

        // A mão ficou vazia; qualquer destaque de slot deixa de fazer
        // sentido até a próxima vez que Al pegar outro bloco.
        _highlightedSlot?.SetHighlighted(false);
        _highlightedSlot = null;
    }

    private void PlayErrorSound()
    {
        if (errorSound != null)
        {
            _audioSource.PlayOneShot(errorSound);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out DadoBlock block) && !block.IsHeld)
        {
            _nearbyBlocks.Add(block);
        }

        if (other.TryGetComponent(out NumericSlot slot))
        {
            _nearbySlots.Add(slot);
        }

        if (other.TryGetComponent(out ConfirmationLever lever))
        {
            _nearbyLevers.Add(lever);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out DadoBlock block))
        {
            _nearbyBlocks.Remove(block);

            if (block == _highlightedBlock)
            {
                block.SetHighlighted(false);
                _highlightedBlock = null;
            }
        }

        if (other.TryGetComponent(out NumericSlot slot))
        {
            _nearbySlots.Remove(slot);

            if (slot == _highlightedSlot)
            {
                slot.SetHighlighted(false);
                _highlightedSlot = null;
            }
        }

        if (other.TryGetComponent(out ConfirmationLever lever))
        {
            _nearbyLevers.Remove(lever);

            if (lever == _highlightedLever)
            {
                lever.SetHighlighted(false);
                _highlightedLever = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (alMovement == null)
        {
            return;
        }

        Vector3 dropPosition = transform.position + (Vector3)(alMovement.FacingDirection * dropDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dropPosition, dropCheckRadius);
    }
}
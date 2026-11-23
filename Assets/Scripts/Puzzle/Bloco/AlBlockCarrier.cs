using System.Collections.Generic;
using UnityEngine;

public class AlBlockCarrier : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private AlMovement alMovement;

    [Header("Largar Bloco")]
    [SerializeField] private float dropDistance = 0.8f;
    [SerializeField] private float dropCheckRadius = 0.4f;
    [SerializeField] private LayerMask dropObstacleMask;
    [SerializeField] private bool findNearbySpotWhenBlocked = true;

    private readonly HashSet<DadoBlock> _nearbyBlocks = new();
    private readonly HashSet<NumericSlot> _nearbySlots = new();
    private readonly HashSet<ConfirmationLever> _nearbyLevers = new();
    private DadoBlock _highlightedBlock;
    private ConfirmationLever _highlightedLever;
    private DadoBlock _heldBlock;
    public DadoBlock HeldBlock => _heldBlock;

    private void Awake()
    {
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

    private void InsertHeldBlockIntoSlot(NumericSlot slot)
    {
        DadoBlock block = _heldBlock;
        bool inserted = slot.TryInsertBlock(block);

        if (inserted)
        {
            _heldBlock = null;
            _nearbyBlocks.Add(block);
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
            SetLeverHighlight(null);
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
        Vector2 facing = alMovement.FacingDirection;
        Vector3 dropPosition = transform.position + (Vector3)(facing * dropDistance);

        if (IsDropBlocked(dropPosition) && !(findNearbySpotWhenBlocked && TryFindFreeSpot(facing, out dropPosition)))
        {
            return;
        }

        _heldBlock.Drop(dropPosition);
        _heldBlock = null;
    }

    private bool IsDropBlocked(Vector3 position)
    {
        return Physics2D.OverlapCircle(position, dropCheckRadius, dropObstacleMask);
    }

    private static readonly float[] SearchAngles = { 45f, -45f, 90f, -90f, 135f, -135f, 180f };

    private bool TryFindFreeSpot(Vector2 facing, out Vector3 spot)
    {
        for (int ring = 0; ring < 2; ring++)
        {
            float distance = ring == 0 ? dropDistance : dropDistance * 0.5f;

            foreach (float angle in SearchAngles)
            {
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * facing;
                spot = transform.position + (Vector3)(direction * distance);

                if (!IsDropBlocked(spot))
                {
                    return true;
                }
            }
        }

        spot = default;
        return false;
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
}
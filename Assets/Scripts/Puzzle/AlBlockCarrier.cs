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
    private DadoBlock _highlightedBlock;
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
        }

        if (InputManager.Instance == null || !InputManager.Instance.AlInteractPressed)
        {
            return;
        }

        if (_heldBlock != null)
        {
            TryDropHeldBlock();
        }
        else if (_highlightedBlock != null)
        {
            PickupHighlightedBlock();
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
        DadoBlock closest = FindClosestBlock();

        if (closest == _highlightedBlock)
        {
            return;
        }

        _highlightedBlock?.SetHighlighted(false);
        _highlightedBlock = closest;
        _highlightedBlock?.SetHighlighted(true);
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
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out DadoBlock block))
        {
            return;
        }

        _nearbyBlocks.Remove(block);

        if (block == _highlightedBlock)
        {
            block.SetHighlighted(false);
            _highlightedBlock = null;
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
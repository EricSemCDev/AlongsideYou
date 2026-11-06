using System.Collections;
using UnityEngine;

// Rastro de "sprites fantasma": enquanto o Finn se move acima de um limiar
// de velocidade, spawna cópias semi-transparentes do sprite atual dele,
// que dão fade-out e se destroem sozinhas. Separado do FinnMovement de
// propósito — é uma responsabilidade puramente visual, não de movimento.
[RequireComponent(typeof(FinnMovement))]
public class FinnTrailEffect : MonoBehaviour
{
    [Header("Trail")]
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private float spawnInterval = 0.06f;
    [SerializeField] private float ghostLifetime = 0.25f;
    [SerializeField] private float speedThreshold = 0.3f;
    [SerializeField] private Color ghostTint = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color ghostEndTint = new Color(0.8f, 0.1f, 0f, 0f);
    [SerializeField] private float ghostEndScale = 0.4f;
    [SerializeField] private float ghostRiseSpeed = 0.6f;
    [SerializeField] private Material ghostMaterial;

    private FinnMovement _finnMovement;
    private float _spawnTimer;

    private void Awake()
    {
        _finnMovement = GetComponent<FinnMovement>();

        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (_finnMovement.CurrentSpeed < speedThreshold)
        {
            _spawnTimer = 0f;
            return;
        }

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer < spawnInterval)
        {
            return;
        }

        _spawnTimer = 0f;
        SpawnGhost();
    }

    private void SpawnGhost()
    {
        if (targetSpriteRenderer == null || targetSpriteRenderer.sprite == null)
        {
            return;
        }

        var ghost = new GameObject("FinnTrailGhost");
        ghost.transform.SetPositionAndRotation(
            targetSpriteRenderer.transform.position,
            targetSpriteRenderer.transform.rotation
        );
        ghost.transform.localScale = targetSpriteRenderer.transform.lossyScale;

        var ghostRenderer = ghost.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = targetSpriteRenderer.sprite;
        ghostRenderer.flipX = targetSpriteRenderer.flipX;
        ghostRenderer.color = ghostTint;
        ghostRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;
        ghostRenderer.sortingOrder = targetSpriteRenderer.sortingOrder - 1;

        // Material aditivo (sem luz): o rastro brilha mesmo no escuro,
        // como fogo. Pode ser trocado no Inspector (ghostMaterial).
        ghostRenderer.sharedMaterial = ghostMaterial != null
            ? ghostMaterial
            : FinnFlameTrail.GetAdditiveMaterial();

        StartCoroutine(FadeAndDestroy(ghostRenderer));
    }

    private IEnumerator FadeAndDestroy(SpriteRenderer ghostRenderer)
    {
        float elapsed = 0f;
        Color startColor = ghostRenderer.color;
        Vector3 startScale = ghostRenderer.transform.localScale;

        while (elapsed < ghostLifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ghostLifetime;

            // Esquenta -> esfria: cor vai do tint para o tom final e some.
            ghostRenderer.color = Color.Lerp(startColor, ghostEndTint, t);

            // Encolhe e sobe, como o calor de uma chama.
            ghostRenderer.transform.localScale = startScale * Mathf.Lerp(1f, ghostEndScale, t);
            ghostRenderer.transform.position += Vector3.up * (ghostRiseSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(ghostRenderer.gameObject);
    }
}
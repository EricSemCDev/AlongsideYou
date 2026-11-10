using UnityEngine;

// Diz ao shader de contorno ONDE esta o jogador, para o contorno dos objetos
// em destaque aparecer por proximidade: quanto mais perto, mais completo, e
// o lado mais proximo do jogador aparece primeiro. Independente das luzes
// (a luz do Al encolhe com a ansiedade; isto nao).
// Coloque no Al. Usa uma variavel global do shader (_OutlineRevealCenter),
// entao um unico script atende todos os objetos com o material de contorno.
public class OutlineRevealSource : MonoBehaviour
{
    private static readonly int RevealCenterId = Shader.PropertyToID("_OutlineRevealCenter");

    [Tooltip("Deslocamento a partir do pivot (ex: para o centro do corpo).")]
    [SerializeField] private Vector2 offset;

    private void Update()
    {
        Vector3 position = transform.position;
        Shader.SetGlobalVector(RevealCenterId, new Vector4(position.x + offset.x, position.y + offset.y, 0f, 1f));
    }

    private void OnDisable()
    {
        // w = 0: sem fonte, o shader mostra o contorno completo.
        Shader.SetGlobalVector(RevealCenterId, Vector4.zero);
    }
}

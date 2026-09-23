using UnityEngine;

// Lógica pura de qual operador está sendo apontado no menu radial,
// dada a posição do centro do menu e a posição do mouse. Sem
// dependência de UI/Canvas — só matemática de ângulo.
public static class OperatorMenuSelector
{
    // Quadrantes diagonais (divididos pelos eixos vertical/horizontal),
    // sentido horário a partir de cima-direita: + (cima-direita),
    // - (baixo-direita), × (baixo-esquerda), ÷ (cima-esquerda).
    public static char GetSelectedOperator(Vector2 menuCenter, Vector2 pointerPosition)
    {
        Vector2 direction = pointerPosition - menuCenter;

        if (direction == Vector2.zero)
        {
            return '+'; // caso de borda: mouse exatamente no centro, assume cima-direita
        }

        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angleDeg < 0f)
        {
            angleDeg += 360f;
        }

        // 0° = direita, 90° = topo, 180° = esquerda, 270° = baixo
        // (convenção matemática padrão, sentido anti-horário)
        if (angleDeg >= 0f && angleDeg < 90f)
        {
            return '+'; // cima-direita
        }

        if (angleDeg >= 90f && angleDeg < 180f)
        {
            return '/'; // cima-esquerda
        }

        if (angleDeg >= 180f && angleDeg < 270f)
        {
            return '*'; // baixo-esquerda
        }

        return '-'; // baixo-direita (270°-360°)
    }
}
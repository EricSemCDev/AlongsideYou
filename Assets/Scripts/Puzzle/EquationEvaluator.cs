// Avalia uma expressão estritamente da esquerda pra direita, sem
// prioridade de operador (2+3*4 = 20, não 14). Classe pura, sem
// dependência de Unity/cena.
public static class EquationEvaluator
{
    public static double Evaluate(double[] values, char[] operators)
    {
        double result = values[0];

        for (int i = 0; i < operators.Length; i++)
        {
            result = Apply(result, operators[i], values[i + 1]);
        }

        return result;
    }

    private static double Apply(double left, char op, double right)
    {
        switch (op)
        {
            case '+':
                return left + right;
            case '-':
                return left - right;
            case '*':
                return left * right;
            case '/':
                return right != 0 ? left / right : 0; // evita divisão por zero
            default:
                return left;
        }
    }
}
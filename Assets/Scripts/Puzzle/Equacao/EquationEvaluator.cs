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
                return right != 0 ? left / right : 0;
            default:
                return left;
        }
    }
}
public static class InputAccessPolicy
{
    public static bool CanAcceptInput(GameState currentState)
    {
        return currentState == GameState.Playing;
    }
}
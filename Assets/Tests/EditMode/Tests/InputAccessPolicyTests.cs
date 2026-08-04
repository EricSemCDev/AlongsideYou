using NUnit.Framework;

public class InputAccessPolicyTests
{
    [Test]
    public void ShouldAcceptInput_WhenGameIsPlaying()
    {
        bool result = InputAccessPolicy.CanAcceptInput(GameState.Playing);

        Assert.IsTrue(result);
    }

    [Test]
    public void ShouldNotAcceptInput_WhenGameIsInMenu()
    {
        bool result = InputAccessPolicy.CanAcceptInput(GameState.Menu);

        Assert.IsFalse(result);
    }

    [Test]
    public void ShouldNotAcceptInput_WhenGameIsPaused()
    {
        bool result = InputAccessPolicy.CanAcceptInput(GameState.Paused);

        Assert.IsFalse(result);
    }
}
using NUnit.Framework;

public class GameStateMachineTests
{
    [Test]
    public void DeveIniciarNoEstadoMenu()
    {
        var stateMachine = new GameStateMachine();

        Assert.AreEqual(GameState.Menu, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDeMenuParaPlaying()
    {
        var stateMachine = new GameStateMachine();

        bool sucesso = stateMachine.TryTransitionTo(GameState.Playing);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Playing, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDeGameOverParaPlaying()
    {
        var stateMachine = new GameStateMachine(GameState.GameOver);

        bool sucesso = stateMachine.TryTransitionTo(GameState.Playing);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Playing, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDeCompleteParaPlaying()
    {
        var stateMachine = new GameStateMachine(GameState.Complete);

        bool sucesso = stateMachine.TryTransitionTo(GameState.Playing);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Playing, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDePlayingParaPaused()
    {
        var stateMachine = new GameStateMachine(GameState.Playing);

        bool sucesso = stateMachine.TryTransitionTo(GameState.Paused);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Paused, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDePlayingParaGameOver()
    {
        var stateMachine = new GameStateMachine(GameState.Playing);

        bool sucesso = stateMachine.TryTransitionTo(GameState.GameOver);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.GameOver, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDePlayingParaComplete()
    {
        var stateMachine = new GameStateMachine(GameState.Playing);

        bool sucesso = stateMachine.TryTransitionTo(GameState.Complete);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Complete, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDePausedParaPlaying()
    {
        var stateMachine = new GameStateMachine(GameState.Paused);

        bool sucesso = stateMachine.TryTransitionTo(GameState.Playing);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Playing, stateMachine.CurrentState);
    }

    [Test]
    public void DeveTransicionarDePausedParaMenu()
    {
        var stateMachine = new GameStateMachine(GameState.Paused);

        bool sucesso = stateMachine.TryTransitionTo(GameState.Menu);

        Assert.IsTrue(sucesso);
        Assert.AreEqual(GameState.Menu, stateMachine.CurrentState);
    }

    [Test]
    public void NaoDeveTransicionarDeMenuParaGameOver()
    {
        var stateMachine = new GameStateMachine(GameState.Menu);

        bool sucesso = stateMachine.TryTransitionTo(GameState.GameOver);

        Assert.IsFalse(sucesso);
        Assert.AreEqual(GameState.Menu, stateMachine.CurrentState);
    }
}
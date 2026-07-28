using NUnit.Framework;

public class SanityCheckTests
{
    [Test]
    public void PipelineDeTestesEstaFuncionando()
    {
        // Teste intencionalmente trivial - existe so para validar que o
        // Test Runner, o Assembly Definition e o game-ci estao conectados
        // corretamente antes de escrever testes reais.
        int resultado = 2 + 2;
        Assert.AreEqual(4, resultado);
    }
}

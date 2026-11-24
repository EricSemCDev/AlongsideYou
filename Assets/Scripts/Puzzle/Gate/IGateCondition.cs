// Qualquer condição de portão (limiar, paridade, sinal...) implementa
// isso, permitindo que o GateCastActivator pergunte "está satisfeita?"
// sem precisar saber qual tipo específico está usando.
public interface IGateCondition
{
    bool ConditionMet { get; }
}
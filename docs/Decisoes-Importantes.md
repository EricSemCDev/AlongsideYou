# Decisões Importantes

Registro de decisões relevantes tomadas durante o projeto, com data e justificativa.

---

## 06/03/2026

**Nome alterado para Alongside You**
Nome anterior fraco para portfólio. Alongside You captura parceria e superação emocional.

**Bobby renomeado para Finn**
Al + Finn = Final — a parceria é o que leva Al ao fim da jornada.

**Blocos redesenhados como dados com faces giráveis**
Valores fixos eliminariam criatividade. Faces giráveis permitem múltiplas soluções.

**Faíscas-operador como mecânica do Finn**
Finn estava passivo. Coletar operadores torna o J2 agente ativo do puzzle.

**Escuridão definida como estática**
Escuridão dinâmica sem ganho narrativo. Estática com decaimento global entrega a mesma tensão.

---

## 10/03/2026

**Áreas de Lamento e Memórias de Gelo criadas**
Vento e plantas sem conexão narrativa. Novos obstáculos fecham narrativa e mecânica.

**Obstáculos bloqueiam itens, não o caminho principal**
Se obstáculo bloqueia o caminho, um dos jogadores ficaria preso. Itens bloqueados mantém o design cooperativo.

**Corda de luz em vez de indicador numérico**
Número de metros quebrava imersão. Corda é diegética e legível para ambos.

**Puzzle gerado proceduralmente**
Puzzles fixos permitiriam memorização, eliminando o raciocínio matemático.

**Sistema de ansiedade progressiva adicionado**
Professor apontou falta de urgência. Ansiedade cria pressão coerente com a narrativa.

---

## 15/03/2026

**Migração para Unity 2022.3 LTS**
Unity 6 sem imagem Docker no game-ci. 2022.3 LTS tem suporte completo e estável.

---

## 22/05/2026

**Vista lateral flat substituindo platformer convencional**
Mais simples em pixel art. Mantém todas as mecânicas sem alteração.

**Pilares de design definidos**
Professor identificou ausência de filosofia de design explícita.

**Pulo removido dos controles do Al**
Vista lateral flat não exige pulo. Simplifica controles.

---

## 25/06/2026

**Ansiedade muda de derrota para dificuldade crescente**
Professor apontou conflito com Celeste como referência. Estágio 5 paralisa, Finn salva.

**Gamepad adicionado como alternativa para Finn**
Professor apontou risco ergonômico do co-op teclado+mouse. Gamepad resolve o atrito físico.

**Logline narrativa definida**
Professor identificou ausência de narrativa explícita. Al é filho de pais inconsistentes, Finn é amigo imaginário.

---

## 29/06/2026

**16 fases principais + 4 fases secretas substituindo 20 fases lineares**
Sistema de estrelas e fases secretas cria progressão visual e loop de rejogo sem economia interna.

**Skins como recompensa das fases secretas**
Roupa do Al e cor do Finn como personalização — recompensa estética sem impacto em gameplay.

---

## 06/08/2026

**GameManager separado em duas camadas: GameStateMachine e GameManager**
`GameStateMachine` (classe C# pura) viabiliza testes automatizados isolados da engine. `GameManager` (MonoBehaviour singleton) instancia e expõe às cenas. Mantido mesmo após a remoção do TDD por ser boa prática de arquitetura, não exclusiva de teste.

---

## 08/08/2026

**InputManager centraliza Al e Finn, bloqueando input fora do estado Playing**
Evita que os personagens se movam durante menu ou pausa. Centraliza toda leitura de input em um único ponto do código.

---

## 12/08/2026

**Movimento do Finn passa a usar sistema de ponto-alvo (target offset) com Mathf.SmoothDamp**
Clique do mouse e analógico direito alimentam a mesma mecânica. SmoothDamp entrega aceleração e desaceleração suaves em vez de movimento abrupto, com indicador visual do alvo.

---

## 13/08/2026

**TDD removido como processo obrigatório do projeto**
Alinhado com os professores orientadores: para o escopo e prazo do projeto, playtests ao final entregam mais valor que testes automatizados durante o desenvolvimento. A arquitetura resultante do processo de TDD foi mantida.

---

*← [Roadmap](Roadmap) | [Home](Home) →*

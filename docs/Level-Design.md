# Level Design

As fases são estruturadas como **ilhas flutuantes dentro de dungeons fechadas** — fragmentos da mente de Al navegáveis.

## Princípio de Design

O design é **linear com exploração local**: o destino é sempre único (a Fagulha), mas o espaço entre início e fim tem múltiplas ilhas. O caminho principal entre as ilhas é **sempre acessível para ambos os personagens via pontes fixas**.

> Os obstáculos (Gelo e Lamento) bloqueiam **itens do puzzle**, nunca o caminho principal. Se um obstáculo bloqueasse o caminho, um dos jogadores poderia ficar preso — o design cooperativo exige que ambos sempre consigam avançar juntos.

---

## Layouts de Fase

### Bloco 1 e 2 — Fase Simples e Média

Fase simples (A Névoa): 2 ilhas, 1 bloco-dado, 1 faísca-operador em ilha exclusiva do Finn, estação de cálculo central, resultado alvo explícito na plaquinha.

Fase média (O Labirinto): 3 ilhas, 2 blocos-dado, obstáculo de Gelo bloqueando uma faísca-operador, caminho principal sempre livre via pontes fixas.

### Bloco 3 e 4 — Fase Avançada

4+ ilhas, abertura estreita que só Finn atravessa, obstáculos sem valor numérico exibido (apenas `>` ou `<`), expressão com 3 valores (A op B op C), múltiplos blocos e faíscas.

---

## Progressão de Complexidade

| Bloco | Ilhas | Elementos | Obstáculos | Matemática |
|---|---|---|---|---|
| 1 — A Névoa | 2–3 | 1 bloco, 1 faísca | Nenhum | A + B, resultado explícito |
| 2 — O Labirinto | 3 | 2 blocos, 1–2 faíscas | Gelo bloqueia item | A - B ou A × B |
| 3 — O Abismo | 4 | 2 blocos, 2 faíscas | Gelo + abertura estreita | A × B ou A ÷ B, resultado sem número |
| 4 — O Centro | 4–5 | 3 blocos, 2 faíscas | Gelo + Lamento, sem valores | A op B op C |

---

*← [Mecânicas](Mecanicas) | [Controles](Controles) →*

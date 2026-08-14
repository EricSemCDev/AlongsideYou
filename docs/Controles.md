# Controles

## Al — Jogador 1 (Teclado)

| Input | Ação |
|---|---|
| `A` / `D` | Mover para esquerda / direita |
| `W` / `S` | Mover para cima / baixo (onde aplicável) |
| `Espaço` | Pegar / largar bloco-dado |
| `Q` | Girar face do dado para esquerda |
| `E` | Girar face do dado para direita |
| `F` | Confirmar resultado na Estação de Cálculo |
| `ESC` | Abrir menu de pausa |

---

## Finn — Jogador 2

### Padrão (Mouse)

| Input | Ação |
|---|---|
| Mover mouse | Finn segue o cursor com lerp |
| Clique esquerdo | Coletar faísca-operador / interagir |

### Alternativo (Gamepad)

| Input | Ação |
|---|---|
| Analógico direito | Finn segue o analógico com lerp |
| Botão direito | Coletar faísca-operador / interagir |

> O controle alternativo com gamepad existe para resolver o atrito ergonômico do co-op teclado+mouse no mesmo PC — os dois jogadores sentados lado a lado disputando espaço físico.

| Ambos | `ESC` | Abrir menu de pausa |
|---|---|---|

---

## Câmera

Vista lateral 2D com profundidade de camadas — estilo South Park Stick of Truth. Câmera fixa ou com scroll lateral suave seguindo Al. Profundidade criada por sorting layers no Unity — fundo, meio e primeiro plano.

---

## Flow de Menus

| De | Para | Ação |
|---|---|---|
| Menu Principal | Seleção de Fases | Jogar |
| Seleção de Fases | Jogo | Seleciona fase → inicia |
| Jogo | Menu de Pausa | ESC |
| Menu de Pausa | Jogo | Continuar |
| Menu de Pausa | Seleção de Fases | Voltar ao Menu |
| Menu de Pausa | Jogo (reiniciado) | Reiniciar Fase |
| Jogo | Tela de Game Over | Condição de derrota |
| Tela de Game Over | Jogo (reiniciado) | Tentar Novamente |
| Jogo | Tela de Conclusão | Fagulha coletada |
| Tela de Conclusão | Próxima fase | Continuar |

---

*← [Level Design](Level-Design) | [Obstáculos e Derrota](Obstaculos-e-Derrota) →*

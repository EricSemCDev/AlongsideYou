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

## Finn — Jogador 2 (Mouse)

| Input | Ação |
|---|---|
| Mover mouse | Finn segue o cursor com lerp |
| Clique esquerdo | Coletar faísca-operador / interagir com elementos |
| `ESC` | Abrir menu de pausa |

---

## Câmera

Vista lateral 2D com profundidade de camadas — estilo South Park Stick of Truth. Câmera fixa ou com scroll lateral suave seguindo Al. Profundidade criada por sorting layers no Unity — fundo, meio e primeiro plano. Sprites dos personagens são completamente planos (flat) sem perspectiva.

---

## Flow de Menus

| De | Para | Ação |
|---|---|---|
| Menu Principal | Jogo | Jogar → seleciona fase → inicia |
| Jogo | Menu de Pausa | ESC |
| Menu de Pausa | Jogo | Continuar |
| Menu de Pausa | Menu Principal | Voltar ao Menu |
| Menu de Pausa | Jogo (reiniciado) | Reiniciar Fase |
| Jogo | Tela de Game Over | Condição de derrota |
| Tela de Game Over | Jogo (reiniciado) | Tentar Novamente |
| Jogo | Tela de Conclusão | Fagulha coletada |
| Tela de Conclusão | Próxima fase | Continuar |

---

*← [Mecânicas](Mecanicas) | [Obstáculos e Derrota](Obstaculos-e-Derrota) →*

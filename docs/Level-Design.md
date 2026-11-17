# Level Design

As fases são **ilhas de terra no meio de um vazio coberto de névoa**, ligadas por pontes e caminhos. O destino é sempre a Fagulha de Luz, atrás de uma barreira de espinhos. Entre o início e o fim, o mapa espalha blocos, faíscas e tochas de luz.

---

## Princípios

- **Uma conta por fase** — cada fase tem uma Estação de Cálculo e um único puzzle gerado
- **Espaço só do Finn** — espinhos e passagens que só o Finn atravessa guardam faíscas, forçando a divisão de tarefas
- **Iscas** — blocos e faíscas a mais do que a conta precisa obrigam o jogador a escolher, não só a coletar tudo
- **Luz como rota** — tochas marcam zonas seguras; trechos escuros longos aumentam a pressão da ansiedade
- **Uma ideia nova por fase** — decimais, tochas apagando, ordem dos operadores, tentativa única

---

## Como a Dificuldade Muda a Conta

O nível de dificuldade (1 a 10) de cada `PuzzleConfig` controla o gerador:

| Dificuldade | Números de cada espaço | Resultado máximo | Chance de × e ÷ | Negativos |
|---|---|---|---|---|
| até 2 | 1 a 4 | 10 | 20% | Não |
| até 5 | 5 a 9 | 50 | 50% | Não |
| até 7 | 0 a 9 | 100 | 80% | Não |
| 8 a 10 | 0 a 9 | 500 | 80% | Sim |

> Espaços compostos sempre recebem um número de 10 a 99, independente da dificuldade.

A progressão fase a fase está em [Estrutura das Fases](Estrutura-das-Fases).

---

*← [Mecânicas](Mecanicas) | [Controles](Controles) →*

# Obstáculos e Derrota

## Princípio de Design

Os obstáculos **bloqueiam itens do puzzle (blocos-dado ou faíscas-operador), nunca o caminho principal**. O caminho entre ilhas é sempre acessível via pontes fixas — isso garante que ambos os jogadores sempre conseguem avançar juntos até a Fagulha.

---

## Elementos de Fase

### Áreas de Lamento
Zonas úmidas com goteiras — memórias dolorosas de Al que ele evita revisitar.

- Bloqueiam um item (bloco ou faísca) até a condição ser satisfeita
- Para liberar: resultado da equação deve ser `< X`
- Fases avançadas mostram apenas `<` sem o valor numérico

### Memórias de Gelo
Estruturas de gelo que bloqueiam um item — memórias boas que Al esqueceu.

- Derretem quando resultado da equação é `> X`
- Fases avançadas mostram apenas `>` sem o valor numérico

### Checkpoints de Luz
Totens ou lamparinas espalhados pelo mapa.

- Quando acesos por Finn, estabilizam a iluminação ao redor temporariamente

### Faíscas-Operador em Áreas Escuras
Operadores posicionados em regiões de maior escuridão — risco vs. recompensa na coleta.

### Aberturas Estreitas
Passagens que só Finn consegue atravessar. Al encontra rota alternativa enquanto Finn ativa mecanismos do outro lado.

---

## Condições de Derrota

O jogo não tem checkpoints — derrota reinicia a fase inteira.

| Condição | Descrição |
|---|---|
| **Al no escuro** | Al fica em área totalmente escura por tempo demais |
| **Separação de Finn** | Finn se afasta além do raio de distância por tempo demais |
| **Ansiedade — Estágio 5 sem resgate** | Al paralisa no Estágio 5. Se Finn não conseguir iluminá-lo a tempo, é derrota |

> A ansiedade **não é punição direta** — é dificuldade progressiva. A derrota só acontece se a parceria falhar em resgatar Al a tempo. Ver [Mecânicas](Mecanicas) para o sistema completo de estágios.

---

## Tela de Game Over

- Animação narrativa: Al se encolhendo no escuro, Finn tentando alcançá-lo sem conseguir
- Frase aleatória da voz interna do Al
- Botão: **Tentar Novamente**

---

*← [Controles](Controles) | [Estrutura das Fases](Estrutura-das-Fases) →*

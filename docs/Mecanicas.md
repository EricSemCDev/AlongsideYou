# Mecânicas

## Core Loop

```
Explorar → Al pega blocos-dado e gira as faces → Finn coleta faíscas de operador
→ Al encaixa os blocos nos espaços numéricos → Finn acende os operadores nas tochas
→ a estação calcula ao vivo e confirma → conta certa: a barreira de espinhos murcha
→ Al alcança a Fagulha de Luz → tela de resultado com estrelas
```

---

## Loops Secundários

### Ansiedade
No escuro, a ansiedade de Al sobe de estágio; perto de uma luz segura (Finn ou tocha) ela desce. O jogador planeja a rota pensando nas zonas de luz.

### Tochas que apagam
Em algumas fases, as tochas do cenário queimam por um tempo, diminuem e apagam. O Finn reacende ficando cerca de 3 segundos dentro da área da tocha.

### Tentativa e ajuste
Cada combinação completa de blocos e operadores é avaliada uma vez. Se errar, o resultado fica vermelho e o jogador troca peças. Nas fases 9 e 10 só existe uma tentativa: errar reinicia a fase.

---

## Al

- Anda em 8 direções (WASD/setas ou analógico), sem pulo
- Pega **um bloco por vez**, gira as faces (Q/E ou LB/RB) e encaixa num espaço numérico (Espaço ou A)
- Pode tirar um bloco do espaço de volta
- Ao largar no chão, o bloco cai no primeiro ponto livre à frente ou ao redor
- Fica mais lento conforme a ansiedade sobe

---

## Finn

- Segue um ponto-alvo suavizado (`SmoothDamp`): com o mouse, segurar o clique esquerdo faz o Finn ir até o cursor; no controle, segue o analógico
- Voa e atravessa os espinhos
- Coleta faíscas de operador por contato — cada uma vai para o estoque dele
- Perto de uma tocha da estação, abre a **roda de operadores** (clique direito segurado, ou segurar A/Cruz), aponta o operador e solta para acender. A roda mostra quantos de cada ele tem
- A luz dele protege o Al da ansiedade e reacende tochas apagadas

---

## Blocos-Dado

- Cada bloco tem de 1 a 4 faces, conforme a fase
- As faces são geradas a cada carregamento da fase; os dígitos da solução ficam espalhados pelos blocos
- Fases com mais blocos do que espaços têm blocos de isca
- **Espaço composto:** recebe dois blocos (dezena e unidade) e forma um número de 10 a 99

---

## Estação de Cálculo

Feita de elementos do mundo, não é uma interface separada.

- **Espaços numéricos** — onde o Al encaixa os blocos, em ordem
- **Tochas de operador** — onde o Finn acende os operadores, entre os espaços
- **Painel de resultado** — mostra a conta ao vivo; fica verde ao acertar e vermelho ao errar
- **Tocha de resultado esperado** — mostra o alvo da conta
- Confirma sozinha quando todos os espaços e tochas estão preenchidos (uma vez por combinação)

### Regras da conta

- Avaliada **da esquerda para a direita, sem prioridade de operador**: `2 + 3 × 4 = 20` (e não 14)
- Divisão só aparece quando é exata e diferente de zero
- Resultados negativos só nas fases de dificuldade 8 ou mais
- O alvo é sorteado a cada carregamento da fase — decorar a resposta não funciona

---

## Puzzle Procedural

A cada carregamento, o `PuzzleGenerator` cria uma equação nova a partir do `PuzzleConfig` da fase (blocos, faces, espaços, espaços compostos, operadores e dificuldade). Como o alvo sempre vem de uma conta que de fato existe, **toda fase tem solução**. Detalhes em [Stack Técnica](Stack-Tecnica).

---

## Sistema de Ansiedade

A ansiedade é **dirigida pela luz**: se o Al fica fora do alcance de qualquer luz segura (o Finn ou uma tocha acesa) pelo tempo da fase, sobe um estágio; se fica perto de uma luz pelo tempo de recuperação, desce um. A luz pessoal do Al não conta como segura. Os tempos e as velocidades ficam num `AnxietyConfig` por fase.

| Estágio | Efeito no Al (fases 1 a 5) |
|---|---|
| 1 — Normal | Velocidade 100%, cor normal |
| 2 — Inquieto | Velocidade 90%, começa a escurecer, a luz pessoal encolhe e treme mais |
| 3 — Ansioso | Velocidade 70%, mais escuro |
| 4 — Em pânico | Velocidade 50%, quase preto |
| 5 — Paralisado | Movimento travado: derrota, a fase reinicia |

> Nas fases 6, 8 e 9 as velocidades caem mais rápido (100% → 70% → 50% → 30% → 0%) e a ansiedade sobe em 1 a 4 segundos no escuro. Na Fase 9 ela não recua: a luz só impede que suba. As demais fases usam a configuração padrão (sobe a cada 8 s no escuro, desce a cada 1 s na luz).

---

## Estrelas e Progressão

| Fases | ⭐⭐⭐ | ⭐⭐ | ⭐ |
|---|---|---|---|
| Prólogo e fases 1 a 4 | até 30 s | até 60 s | até 90 s |
| Fases 5 a 10 | até 45 s | até 90 s | até 150 s |

- Acima do tempo de 1 estrela, a fase conta como concluída com 0 estrelas
- O cronômetro para durante a pausa e o tutorial
- A melhor nota de cada fase fica salva
- Na tela de resultado as estrelas enchem uma a uma, com som; depois aparecem **Próxima fase** e **Refazer**
- As fases seguem em ordem fixa; ao terminar a Fase 10, o jogo abre os créditos

---

## Modos de Jogo

| Modo | Como funciona |
|---|---|
| **Solo** | Um jogador. No teclado e mouse, controla o Al pelo teclado e o Finn pelo mouse ao mesmo tempo. No controle, um personagem por vez, trocando com Y/Triângulo (uma "alma de luz" salta de um para o outro). |
| **Co-op** | Dois jogadores no mesmo PC: teclado (Al) e mouse (Finn), ou controles (o primeiro é do Al e o segundo do Finn). A troca fica desligada. |

---

*← [Personagens e Narrativa](Personagens-e-Narrativa) | [Level Design](Level-Design) →*

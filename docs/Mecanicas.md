# Mecânicas

## Al — Jogador 1 (Teclado)

- Movimentação padrão de platformer 2D: andar e pular
- Carrega blocos-dado e gira as faces com botão dedicado enquanto caminha
- Decide o valor do bloco durante o trajeto — chega na estação já com o valor escolhido
- Encaixa blocos-dado nos slots de número da Estação de Cálculo
- Não consegue entrar em áreas com luz abaixo do limite mínimo
- Efeito de pânico: animação curta de recuo com mão na cabeça, sem input durante a animação
- Pânico progressivo quando Finn ultrapassa o raio de distância — derrota se a separação persistir
- Fica mais lento conforme o estágio de ansiedade aumenta
- Resolver uma equação corretamente alivia o estágio de ansiedade atual

---

## Finn — Jogador 2 (Mouse)

- Segue o cursor do mouse com lerp — movimento fluido com atraso orgânico
- Tem colisão física com o cenário: animação de espatifar e se reconstruir ao colidir com paredes
- Voa — sem gravidade, sem necessidade de pular
- Não é afetado pela escuridão
- Ilumina o ambiente ao redor criando zona segura onde Al pode avançar
- Tem reserva de energia que drena enquanto ilumina — precisa recarregar em fontes do mapa (tochas, cristais, runas)
- Coleta faíscas-operador espalhadas pelo mapa (`+`, `-`, `×`, `÷`) e as posiciona nos slots da Estação de Cálculo
- Passa por aberturas pequenas inacessíveis para Al — mecânica de divisão de espaço e coordenação
- O raio de iluminação diminui conforme o estágio de ansiedade do Al aumenta

---

## Blocos-Dado

- Carregados pelo Al, formato visual de dado em pixel art
- 4 faces nas fases iniciais (Blocos 1 e 2), 6 faces nas fases avançadas (Blocos 3 e 4)
- Girados com botão dedicado enquanto Al caminha — decisão tomada durante o trajeto
- Faces geradas proceduralmente — pelo menos uma face correta por bloco
- Intervalos de valor por face — mais de uma solução válida por puzzle
- Blocos no escuro por tempo demais retornam ao ponto de origem

---

## Estação de Cálculo

O elemento central do puzzle. Montada cooperativamente pelos dois jogadores.

- Al encaixa blocos-dado nos slots de número
- Finn posiciona faíscas-operador nos slots de operador
- Preview do resultado exibido antes de confirmar — sem punição por tentar
- Confirmação dispara reação no mapa: ponte, luz, desbloqueio, abertura de passagem
- Cálculo sempre da esquerda para a direita, sem prioridade de operações

### Modos por dificuldade

| Modo | Comportamento |
|---|---|
| Fases iniciais | Resultado alvo explícito exibido na estação |
| Fases avançadas | Resultado alvo removido — tochas acendem conforme o resultado se aproxima |

### Integração com obstáculos

O resultado da equação também determina se obstáculos do mapa são superados:

| Obstáculo | Condição |
|---|---|
| Memórias de Gelo | Resultado `>` X (indicado no obstáculo) |
| Áreas de Lamento | Resultado `<` X (indicado no obstáculo) |

Nas fases avançadas, o valor X é omitido — aparece apenas o símbolo `>` ou `<`.

---

## Sistema de Puzzle Procedural

A cada carregamento de fase, o sistema gera um puzzle único:

- Resultado alvo gerado aleatoriamente dentro de um range de dificuldade
- Faces de cada bloco-dado geradas com pelo menos uma face que participa de uma solução válida
- Faíscas-operador no mapa selecionadas para garantir que existe pelo menos uma combinação que resolve a equação
- Quantidade mínima de soluções válidas parametrizável por fase via ScriptableObject

> Isso garante que o jogador nunca memoriza a solução — cada repetição da fase exige novo raciocínio matemático.

---

## Sistema de Ansiedade Progressiva

Conforme o tempo passa sem resolver o puzzle central da fase, a ansiedade do Al aumenta em estágios:

| Estágio | Descrição | Efeito Mecânico |
|---|---|---|
| 1 — Normal | Estado inicial da fase | Sem penalidades |
| 2 — Inquieto | Al começa a sentir a pressão | Animações de nervosismo, Al levemente mais lento |
| 3 — Ansioso | A ansiedade se manifesta fisicamente | Al mais lento, raio de Finn começa a diminuir |
| 4 — Em Pânico | Al está no limite | Al muito lento, blocos somem mais rápido, Finn com raio mínimo |
| 5 — Consumido | A ansiedade venceu | Derrota — reinicia a fase |

> Resolver uma equação corretamente reduz o estágio de ansiedade atual, reforçando que a matemática é a ferramenta de superação.

---

## Escuridão

- Áreas escuras são estáticas — barreiras físicas fixas no mapa
- Ambiente hostil passivo — pressiona sem perseguir ativamente
- Luz ambiente decai globalmente em todas as fases — pressão constante crescente
- Checkpoints de luz estabilizam a iluminação ao redor temporariamente
- Finn precisa recarregar energia em fontes do mapa para manter a iluminação ativa

---

## Raio de Distância Al-Finn

- Al entra em pânico progressivo se Finn se afastar além do raio máximo
- Corda de luz visível entre Al e Finn quando estão se aproximando do limite
- Corda fica vermelha quando a distância está crítica
- Derrota se a separação persistir por tempo demais
- O raio diminui conforme o estágio de ansiedade aumenta
- Implementação: `LineRenderer` no Unity com lógica de cor baseada na distância

---

*← [Personagens e Narrativa](Personagens-e-Narrativa) | [Obstáculos e Derrota](Obstaculos-e-Derrota) →*

# Mecânicas

## Core Loop

```
Explorar → Posicionar Finn (luz) → Coletar blocos-dado (Al)
→ Girar face do dado → Montar expressão na Estação de Cálculo
→ Confirmar resultado → Mundo reage → Alcançar a Fagulha de Luz
```

---

## Loops Secundários

### Loop de Energia do Finn
Finn ilumina → energia drena → busca fonte de recarga (tocha, cristal, runa) → recarrega → volta a iluminar.

### Loop de Recuperação de Ansiedade
Tempo passa sem resolver puzzle → ansiedade de Al aumenta de estágio → jogadores resolvem equação → ansiedade alivia → ciclo reinicia.

### Loop de Mini Puzzle de Obstáculo
Al e Finn encontram obstáculo bloqueando um item → montam equação com condição `> X` ou `< X` → obstáculo abre → item acessível → avançam.

---

## Al — Jogador 1 (Teclado)

- Movimentação lateral 2D: WASD
- Carrega blocos-dado e gira as faces (Q/E) enquanto caminha
- Decide o valor durante o trajeto — chega na estação com o valor escolhido
- Encaixa blocos nos slots da Estação de Cálculo (Espaço)
- Não consegue entrar em áreas com luz abaixo do limite mínimo
- Efeito de pânico: recuo com mão na cabeça, sem input durante a animação
- Fica mais lento conforme o estágio de ansiedade aumenta
- Resolver equação alivia o estágio de ansiedade atual

---

## Finn — Jogador 2 (Mouse ou Gamepad)

- **Padrão:** segue o cursor do mouse com lerp — movimento fluido com atraso orgânico
- **Alternativo:** segue o analógico direito do gamepad, com aceleração e desaceleração suaves
- Colisão física com o cenário: animação de espatifar e se reconstruir
- Voa — sem gravidade
- Não é afetado pela escuridão
- Ilumina o ambiente criando zona segura onde Al pode avançar
- Reserva de energia que drena enquanto ilumina — recarrega em fontes do mapa
- Coleta faíscas-operador (`+`, `-`, `×`, `÷`) com clique esquerdo ou botão do gamepad
- Passa por aberturas pequenas inacessíveis para Al

---

## Blocos-Dado

- Carregados pelo Al, formato visual de dado em pixel art
- 4 faces nas fases iniciais, 6 faces nas fases avançadas
- Girados com Q/E enquanto Al caminha — decisão durante o trajeto
- Faces geradas proceduralmente — pelo menos uma face correta por bloco
- Blocos no escuro por tempo demais retornam ao ponto de origem

---

## Estação de Cálculo

Composta por elementos do mundo — não é uma interface separada.

- **Pedestal de número** — Al encaixa o bloco-dado aqui
- **Tocha de operador** — Finn acende com a faísca coletada
- **Plaquinha** — exibe o resultado alvo (fases iniciais) ou apenas `>` / `<` (fases avançadas)
- Preview do resultado antes de confirmar — sem punição por tentar
- Cálculo sempre da esquerda para direita, sem prioridade de operações
- Confirmar dispara reação no mapa: ponte, luz, desbloqueio, passagem

---

## Sistema de Puzzle Procedural

A cada carregamento de fase, o sistema gera um puzzle único:

- Resultado alvo gerado aleatoriamente dentro de um range de dificuldade
- Faces de cada bloco geradas com pelo menos uma que participa de uma solução válida
- Faíscas-operador selecionadas para garantir ao menos uma combinação que resolve
- Parâmetros configuráveis via ScriptableObject por fase

> O jogador nunca memoriza a solução — cada repetição exige novo raciocínio matemático.

---

## Sistema de Ansiedade como Dificuldade Crescente

A ansiedade **não é condição de derrota direta** — é dificuldade progressiva. No Estágio 5, Al paralisa por alguns segundos. Se Finn iluminar Al nesse momento, ele se recupera e volta ao Estágio 4. Só há derrota se Finn não conseguir chegar a tempo.

> A parceria literalmente salva Al da ansiedade.

| Estágio | Descrição | Efeito Mecânico |
|---|---|---|
| 1 — Normal | Estado inicial | Sem penalidades |
| 2 — Inquieto | Al começa a sentir a pressão | Animações de nervosismo, levemente mais lento |
| 3 — Ansioso | A ansiedade se manifesta | Al mais lento, raio de Finn começa a diminuir |
| 4 — Em Pânico | Al está no limite | Al muito lento, blocos somem mais rápido, Finn com raio mínimo |
| 5 — Paralisado | A ansiedade consumiu Al | Al imóvel. Finn pode salvar iluminando Al. Sem resgate = derrota. |

---

## Sistema de Estrelas e Progressão

| Estrelas | Critério |
|---|---|
| ⭐ | Completou a fase |
| ⭐⭐ | Completou dentro do tempo médio |
| ⭐⭐⭐ | Completou dentro do tempo ótimo |

Ao conseguir 3 estrelas em todas as 4 fases de um bloco, a **fase secreta** daquele bioma é desbloqueada. As fases secretas são significativamente mais difíceis e recompensam com **skins exclusivas**: roupa alternativa para Al e cor de chama diferente para Finn.

---

## Raio de Distância e Corda de Luz

- Al entra em pânico progressivo se Finn se afastar além do raio máximo
- Corda de luz visível entre os dois ao se aproximarem do limite
- Corda fica vermelha quando a distância é crítica
- Derrota se a separação persistir por tempo demais
- O raio diminui conforme o estágio de ansiedade aumenta

---

## Escuridão

- Áreas escuras estáticas — barreiras físicas fixas no mapa
- Ambiente hostil passivo — pressiona sem perseguir
- Luz ambiente decai globalmente em todas as fases
- Checkpoints de luz estabilizam a iluminação ao redor temporariamente

---

*← [Personagens e Narrativa](Personagens-e-Narrativa) | [Level Design](Level-Design) →*

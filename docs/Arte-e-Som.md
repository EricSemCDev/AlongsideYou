# Arte e Som

## Direção de Arte

- Pixel art flat — sprites 2D sem perspectiva nos personagens
- Vista lateral com profundidade de camadas — sorting layers no Unity
- Sistema de luz 2D do Universal Render Pipeline — luz pontual no Finn
- Escuridão como camada sobreposta — luz do Finn cria recorte circular
- Placeholders geométricos durante o desenvolvimento, arte final nas últimas semanas
- Ferramenta principal: **Aseprite**

---

## Referências Visuais

| Referência | Inspiração |
|---|---|
| South Park: Stick of Truth | Estilo visual flat, sprites 2D laterais, personagens recortados no cenário |
| Hollow Knight | Atmosfera escura, paleta fria, mundo que comunica emoção sem texto |
| Celeste | Fluidez de animação e game feel como direção de qualidade |
| Moonleap | Estrutura de puzzle cooperativo — referência mecânica principal |
| The Binding of Isaac | Pixel art expressivo com personagens pequenos e legíveis |
| Rotom (Pokémon) | Referência de character design para Finn: chama com rosto expressivo |

---

## Animações

### Al

| Animação | Loop | Descrição |
|---|---|---|
| Idle | Sim | Respiração suave, leve balanço — comunica inquietação mesmo parado |
| Andar | Sim | Caminhada lateral com bloco ou sem bloco na mão |
| Pegar bloco | Não | Al se abaixa e levanta com o bloco nas mãos |
| Girar dado | Não | Mãos giram o bloco, faces mudam visivelmente |
| Encaixar bloco | Não | Al empurra o bloco no slot do pedestal |
| Pânico | Não | Recuo com mão na cabeça — sem input disponível durante |
| Inquieto (estágio 2) | Sim | Movimentos nervosos, olha para os lados |
| Ansioso (estágio 3) | Sim | Postura curvada, passos mais lentos |
| Em pânico (estágio 4) | Sim | Tremor visível, movimentação muito lenta |
| Consumido (estágio 5) | Não | Al se encolhe no chão — animação de derrota |

### Finn

| Animação | Loop | Descrição |
|---|---|---|
| Idle | Sim | Chama oscila suavemente, expressão sorridente |
| Voar | Sim | Chama inclina levemente na direção do movimento |
| Coletar faísca | Não | Pulso de luz ao absorver o operador |
| Colidir com parede | Não | Espatifa e se reconstrói — leve e cômico |
| Energia baixa | Sim | Chama encolhe, cor muda para azul frio |
| Recarregando | Não | Pulso de calor ao absorver a fonte de luz |

---

## Áudio

| Som | Onde usar | Loop | Descrição |
|---|---|---|---|
| Trilha — A Névoa | Bioma 1 (fases 1–5) | Sim | Levemente melancólica, etérea, sons de água distante |
| Trilha — O Labirinto | Bioma 2 (fases 6–10) | Sim | Mais tensa, grave, ecos de pedra |
| Trilha — O Abismo | Bioma 3 (fases 11–15) | Sim | Minimalista, frequências baixas, desorientador |
| Trilha — O Centro | Bioma 4 (fases 16–20) | Sim | Etéreo, quase silencioso, bells e pad suave |
| Progressão de ansiedade | Estágios 2 a 5 | Sim | Camadas de tensão sobre a trilha base |
| Encaixar bloco-dado | Al encaixa no slot | Não | Clique satisfatório com peso de encaixe |
| Confirmar equação correta | Resultado validado | Não | Recompensador, leve e positivo |
| Coletar Fagulha | Fim de fase | Não | Clímax suave, expansivo |
| Pânico do Al | Efeito de pânico | Não | Som curto e tenso, distorção leve |
| Finn colidindo | Finn bate na parede | Não | Leve e cômico — reforça personalidade |
| Recarregar Finn | Finn absorve fonte | Não | Agradável, brilhante, reforço positivo |
| Game Over | Tela de derrota | Não | Suave e triste — sem punição sonora |

---

*← [Estrutura das Fases](Estrutura-das-Fases) | [Stack Técnica](Stack-Tecnica) →*

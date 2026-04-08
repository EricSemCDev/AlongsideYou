# Obstáculos e Derrota

## Elementos de Fase

### Áreas de Lamento

Zonas úmidas com goteiras — representam memórias dolorosas de Al, assuntos que ele evita revisitar.

**Mecânica:**
- Reduzem o raio de luz de Finn, exigindo posicionamento mais preciso
- Para atravessar, a equação deve produzir resultado `< X`
- O valor X é indicado visualmente no próprio obstáculo
- Fases avançadas mostram apenas `<` sem o valor numérico

**Afeta:** Finn / Equação

---

### Memórias de Gelo

Estruturas de gelo que bloqueiam o caminho — representam memórias boas que Al esqueceu.

**Mecânica:**
- Derretem quando a equação produz resultado `> X`
- O valor X é indicado visualmente no próprio obstáculo
- Finn mantém a luz enquanto Al atravessa a passagem aberta
- Só Al pode atravessar — Finn não participou dessas memórias
- Fases avançadas mostram apenas `>` sem o valor numérico

**Afeta:** Al / Finn / Equação

---

### Checkpoints de Luz

Totens ou lamparinas espalhados pelo mapa.

**Mecânica:**
- Quando acesos por Finn, viram pontos seguros
- Estabilizam a iluminação ao redor temporariamente
- Reduzem a pressão do decaimento de luz naquela área

**Afeta:** Ambos

---

### Faíscas-Operador em Áreas Escuras

Faíscas com operadores matemáticos posicionadas em regiões de maior escuridão.

**Mecânica:**
- Finn precisa se aventurar em áreas mais escuras para coletá-las
- Risco vs. recompensa — operadores mais valiosos ficam em posições mais perigosas
- Força decisões de priorização entre os jogadores

**Afeta:** Finn

---

### Aberturas Estreitas

Passagens no cenário que só Finn consegue atravessar.

**Mecânica:**
- Al precisa encontrar rota alternativa enquanto Finn passa pelo estreito
- Finn ativa mecanismos ou coleta itens do outro lado
- Força separação momentânea — tensiona o raio de distância

**Afeta:** Finn / Al

---

## Condições de Derrota

O jogo não tem checkpoints — derrota reinicia a fase inteira. Com poucos fatores de falha, cada condição tem peso real.

| Condição | Descrição |
|---|---|
| **Al no escuro** | Al fica em área totalmente escura por tempo demais |
| **Separação de Finn** | Finn se afasta além do raio de distância por tempo demais |
| **Ansiedade consumida** | Sistema de ansiedade atinge o Estágio 5 — Al consumido |

---

## Tela de Game Over

- Animação narrativa: Al se encolhendo no escuro, Finn tentando alcançá-lo sem conseguir
- Frase aleatória da voz interna do Al — escrita pelo desenvolvedor antes da implementação
- Botão: **Tentar Novamente**

---

*← [Mecânicas](Mecanicas) | [Estrutura das Fases](Estrutura-das-Fases) →*

# Decisões Importantes

Registro das decisões do projeto, da concepção à entrega, com data e justificativa. As datas de agosto em diante vêm do histórico do repositório.

---

## 06/03/2026

**Nome alterado para Alongside You**
Nome anterior fraco para portfólio. Alongside You captura parceria e superação emocional.

**Bobby renomeado para Finn**
Al + Finn = Final — a parceria é o que leva Al ao fim da jornada.

**Blocos redesenhados como dados com faces giráveis**
Valores fixos eliminariam a criatividade. Faces giráveis permitem escolher.

**Faíscas de operador como mecânica do Finn**
O Finn estava passivo. Coletar operadores o torna agente do puzzle.

---

## 10/03/2026

**Puzzle gerado proceduralmente**
Puzzles fixos permitiriam decorar a resposta, eliminando o raciocínio matemático.

**Sistema de ansiedade progressiva adicionado**
Professor apontou falta de urgência. A ansiedade cria pressão coerente com a narrativa.

---

## 15/03/2026

**Migração para Unity 2022.3 LTS**
Unity 6 sem imagem Docker no game-ci. 2022.3 LTS tem suporte completo e estável.

---

## 22/05/2026

**Vista 2D flat, sem pulo**
Mais simples em pixel art e mantém todas as mecânicas.

**Pilares de design definidos**
Professor identificou ausência de filosofia de design explícita.

---

## 25/06/2026

**Ansiedade como dificuldade crescente, não derrota imediata**
Professor apontou conflito com Celeste como referência.

**Controle como alternativa para o Finn**
Professor apontou risco ergonômico do co-op teclado + mouse.

**Logline narrativa definida**
Professor identificou ausência de narrativa explícita. Al é filho de pais inconsistentes, Finn é o amigo que ele criou.

---

## 12/08/2026

**Movimento do Finn por ponto-alvo com `Mathf.SmoothDamp`**
Mouse e analógico alimentam a mesma mecânica, com aceleração e desaceleração suaves.

---

## 13/08/2026

**TDD removido como processo obrigatório**
Alinhado com os professores orientadores: para o escopo e prazo do projeto, playtests entregam mais valor que testes automatizados.

---

## 01/09/2026

**Modos Solo e Co-op**
Permite jogar sozinho e testar sem uma segunda pessoa.

---

## 02/09/2026

**Roda de operadores (menu radial) para o Finn**
Escolher entre vários operadores coletados de forma rápida e visual.

---

## 07/09/2026

**Resultado ao vivo na Estação de Cálculo**
O jogador vê o efeito de cada peça e ajusta a conta sem adivinhar.

---

## 21/09/2026

**Ansiedade dirigida pela luz**
Sobe no escuro e desce perto do Finn ou das tochas: dá função à luz e reforça a parceria.

**Gerador da equação para frente com validação, e equação de segurança**
Mesmo efeito da geração reversa sugerida (sempre há solução), com regras mais simples de controlar por dificuldade.

**Derrota no Estágio 5 reinicia a fase**
O resgate pelo Finn não coube no prazo; reiniciar mantém a regra clara.

---

## Outubro de 2026

**Prólogo com tutorial guiado e ícones de botão (17/10)**
Feedback do beta: jogo confuso e operadores difíceis de ver.

**Suporte a controles Xbox, PlayStation e Switch (17/10)**
Conforto no co-op e acessibilidade.

**Tutorial pode ser pulado segurando TAB ou SELECT; falas do Prólogo revisadas (22/10)**
Playtest: parte dos jogadores achou as instruções do começo longas.

**Confirmação automática da conta**
A alavanca de confirmação adicionava um passo sem ganho de jogabilidade.

---

## 06/11/2026

**Estado só muda depois de a cena carregar; gerador usa cópia do `PuzzleConfig`**
Evita personagens andando durante o fade e evita gravar o resultado sorteado no asset.

**Velocidade base do Al de 2 para 2,5**
Playtest: 6 de 10 jogadores acharam o Al lento.

---

## 10/11/2026

**`GameStateMachine` incorporada ao `GameManager`**
Com o fim dos testes automatizados, a separação em duas camadas deixou de ter função.

**Estrelas por tempo e tela de resultado**
Recompensa o domínio da fase e convida a refazer.

**Escopo final de 10 fases**
Prazo: melhor 10 fases polidas do que 20 incompletas.

---

## 13/11/2026

**Um único bioma e corte dos obstáculos `>`/`<`, placas e corda de luz**
Tempo curto: foco nas mecânicas já prontas e reaproveitamento dos mapas.

---

## 17/11/2026

**Fases secretas e skins removidas**
Sem tempo para construí-las; o código foi retirado para não deixar sistema morto.

**Música única em loop e sons por evento**
Música contínua sem cortes entre fases; sons centralizados num asset.

**Créditos com rolagem ao fim da última fase**
Fecha a jornada e credita os recursos de terceiros.

**Largar bloco procura um ponto livre ao redor**
Antes o bloco só caía à frente e falhava perto de paredes.

---

*← [Roadmap](Roadmap) | [Créditos](Creditos) →*

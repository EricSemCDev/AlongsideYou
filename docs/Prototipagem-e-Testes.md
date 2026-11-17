# Prototipagem e Testes

## Prototipagem

Os protótipos foram construídos em sequência, cada um validando uma parte do jogo antes da seguinte. Datas do histórico do repositório.

| Protótipo | Período | Objetivo | Resultado |
|---|---|---|---|
| Movimento e colisão | ago/2026 | Validar o Al (teclado) e o Finn (mouse e controle) numa cena simples | Al com corpo físico e colisão; Finn seguindo um ponto-alvo suavizado, igual para mouse e analógico |
| Blocos-dado | ago/2026 | Pegar, largar e girar faces durante o trajeto | Funcionou; destaque de proximidade adicionado para mostrar o bloco que será pego |
| Estação de Cálculo | set/2026 | Encaixe de blocos, colocação de operadores e cálculo | Roda de operadores, resultado ao vivo e validação da tentativa |
| Ansiedade e luz | set/2026 | Testar estágios e efeitos | Primeiro só por tempo; trocado para "sobe no escuro, desce na luz", que deu sentido às tochas e ao Finn |
| Gerador de equações | set/2026 | Levar o Equation Generator para a Unity | Equações válidas a cada carregamento, com equação de segurança |
| Beta jogável | out/2026 | Fases em sequência, com arte, luz e controle | Feedback de jogo confuso e operadores difíceis de ver: originou o Prólogo, os ícones de botão e o destaque nos objetos |
| Versão final (Gold) | nov/2026 | Fechar o jogo para entrega | 10 fases, estrelas, sons, música e créditos |

---

## Planejamento de Playtests

### Frente 1 — Jogadores com experiência em puzzle
- Entendimento do loop (blocos, roda de operadores e estação) após o Prólogo
- Conforto da assimetria teclado + mouse e do modo Solo
- Leitura da regra da esquerda para a direita nas fases com três números ou mais
- Equilíbrio de dificuldade e justiça dos tempos de estrela

### Frente 2 — Pessoas sem experiência em jogos
- Conclusão do Prólogo e das fases 1 e 2 sem ajuda
- Identificação com o tema da ansiedade
- Sensação de justiça nas derrotas por ansiedade e por tentativa única

---

## Primeiro Feedback do Beta (outubro de 2026)

| Problema observado | Melhoria feita |
|---|---|
| O jogo parecia confuso no começo | Prólogo com tutorial guiado pelo Finn, câmera mostrando cada elemento |
| Os operadores eram difíceis de enxergar | Destaque e contorno nos objetos, ícones de botão no mundo |
| Jogar com controle | Suporte a Xbox, PlayStation e Switch, menus navegáveis e troca de personagem no Solo |

---

## Playtest com Formulário (após o beta)

Dez pessoas jogaram a versão beta e responderam ao Formulário de Playtest do curso (anônimo, em papel), com notas de 1 (discordo totalmente) a 5 (concordo totalmente) e perguntas abertas. Os ajustes que vieram dele já estão na versão final.

### Perfil

| Pergunta | Respostas |
|---|---|
| Quem é | 6 colegas de turma e 4 pessoas de fora da faculdade |
| Joga puzzles desse gênero | 8 não, 2 sim |
| Frequência com que joga | 2 raramente, 2 todo mês, 4 toda semana, 2 todo dia |
| Até onde chegou | 5 terminaram o jogo, 4 completaram uma ou mais fases, 1 jogou uma parte |
| Tempo de jogo | 3 de 5 a 10 min, 6 de 10 a 20 min, 1 mais de 20 min |

### Notas (média de 1 a 5)

| Afirmação | Média |
|---|---|
| Entendi o objetivo do jogo sem precisar de ajuda | 4,3 |
| Os controles responderam como eu esperava | 4,9 |
| As mecânicas do jogo são interessantes | 4,8 |
| A dificuldade foi adequada | 4,7 |
| O enredo do jogo me interessou (9 respostas) | 4,6 |
| O jogo tem algo de original | 4,7 |
| O visual do jogo me agradou | 4,9 |
| Me diverti jogando | 4,9 |

### O que os jogadores disseram

| Tema | Quantos citaram | Exemplos |
|---|---|---|
| Al lento | 6 de 10 | "Aumentar a velocidade do garoto"; "velocidade do personagem principal"; um pediu um botão de correr |
| Começo difícil de entender | 4 de 10 | Não perceberam que dava para girar os blocos (2); demoraram para entender como os operadores entram na conta (2) |
| Tutorial longo | 2 de 10 | "As instruções no começo são um pouco maçantes"; "não gosto de ler tutorial" |
| Luz e tochas pouco claras | 2 de 10 | Não sabiam que o Finn ilumina, reacende tochas e protege do escuro; difícil identificar as tochas de operador na parte final |
| Muitos blocos confundem | 1 de 10 | Perdido quando havia mais de 3 blocos |
| Travamento | 2 de 10 | "O jogo travou e perdi o progresso"; "na última fase o jogo travou" |
| Sugestão de interface | 1 de 10 | Mostrar a expressão inteira no topo da tela |

**Pontos altos citados:** desafios que fazem pensar em soluções diferentes, variedade de operações e progressão entre as fases, controlar os dois personagens e alternar entre eles, a arte, a agilidade para não ficar no escuro e a última fase, considerada desafiadora.

### O que mudou por causa do playtest

| Achado | Mudança no jogo | Quando |
|---|---|---|
| Al lento (6 de 10) | Velocidade base do Al de 2 para 2,5 | 06/11/26 |
| Tutorial longo (2 de 10) | Falas do Prólogo revisadas e opção de pular o tutorial segurando TAB ou SELECT | 22/10/26 |
| Giro dos blocos e operadores pouco percebidos | Ícones de botão no mundo ajustados (por exemplo, "girar" aparece enquanto o Al segura o bloco) | 22/10/26 |
| Travamento | Correções de estabilidade: troca de estado só depois de a cena carregar e proteção contra travamento do gerador | nov/26 |
| Ver a expressão inteira | Não implementado: exigiria uma interface nova (ideia para o futuro) | — |

O confronto com as hipóteses de design está em [Pilares e Hipóteses](Pilares-e-Hipoteses).

---

*← [Stack Técnica](Stack-Tecnica) | [Roadmap](Roadmap) →*

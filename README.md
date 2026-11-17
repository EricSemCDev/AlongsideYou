# Alongside You

> *LUZ. LÓGICA. CORAGEM.*

Um jogo cooperativo onde dois jogadores controlam Al e Finn para usar luz e matemática e atravessar as paisagens da mente de Al.

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![License](https://img.shields.io/badge/Código-MIT-blue)
![Assets](https://img.shields.io/badge/Assets-CC%20BY--NC--SA-green)
![CI](https://github.com/EricSemCDev/AlongsideYou/actions/workflows/main.yml/badge.svg)

---

## Sobre o Projeto

**Alongside You** é um puzzle cooperativo local com vista lateral 2D flat, desenvolvido como projeto de portfólio para o PAC Extensionista VII — Engenharia de Software — Católica SC.

Joga-se em **Solo** (um jogador alterna entre os dois personagens) ou em **Co-op local** (dois jogadores no mesmo PC, com teclado, mouse ou controle). Os personagens são assimétricos:

- **Al (Teclado)** — o protagonista com medo do escuro. Carrega blocos-dado, gira as faces durante o trajeto e resolve puzzles matemáticos
- **Finn (Mouse)** — a luz viva criada pela mente de Al. Ilumina o caminho e coleta operadores matemáticos espalhados pelo mapa

Juntos montam equações na Estação de Cálculo para avançar pelas paisagens da mente de Al — enquanto a ansiedade do personagem vai aumentando e os recursos ficam mais escasos.

---

## Problema e Impacto Social

O projeto aborda dois temas de relevância social:

**Raciocínio lógico-matemático** — estimula o pensamento matemático de forma não escolar, através de puzzles acessíveis e criativos onde não existe uma única resposta correta.

**Saúde mental e ansiedade** — a narrativa trata a ansiedade como tema central. O escuro, os obstáculos e a pressão crescente são metáforas do estado emocional do Al — e a matemática é a ferramenta de superação.

### Validação de Demanda

O projeto foi validado com **22 respondentes** do público-alvo antes do desenvolvimento:

- 95% na faixa de 18 a 24 anos — alinhado com o público esperado
- Interesse médio no conceito: **3,8 / 5**
- Intenção de jogar se gratuito: **3,9 / 5**
- Maioria achou matemática em jogos interessante — *"dependendo de como é apresentada"*

---

## Gameplay

```
Explorar → Posicionar Finn (luz) → Coletar blocos-dado (Al)
→ Girar face do dado → Montar expressão na Estação de Cálculo
→ Confirmar resultado → Mundo reage → Alcançar a Fagulha de Luz
```

- **10 fases** progressivas, mais um Prólogo que apresenta as mecânicas
- **Puzzles procedurais** — cada carregamento de fase gera uma equação diferente
- **Sistema de ansiedade** — quanto mais tempo no escuro, mais o Al deteriora; a luz o acalma
- **Barreiras de espinhos** — só o Finn atravessa, e elas abrem quando a equação da Estação de Cálculo está certa

---

## Stack

| | |
|---|---|
| Engine | Unity 2022.3 LTS |
| Linguagem | C# |
| Arte | Aseprite (pixel art flat) |
| CI/CD | GitHub Actions + game-ci |
| Distribuição | Itch.io (WebGL) + Windows |

---

## Repositórios

| | |
|---|---|
| 🎮 Alongside You (este) | Código-fonte completo do jogo |
| 🧮 [Equation Generator](https://github.com/EricSemCDev/Equation-Generator) | Módulo de geração procedural de equações — algoritmo que garante puzzles únicos e sempre solúveis a cada fase |

---

## Links

| | |
|---|---|
| 🎮 Build jogável | *em breve — Itch.io* |
| 📖 Wiki | [Documentação completa](../../wiki) |
| 🗂️ Docs | [/docs](./docs) |

---

## Documentação

| Documento | Descrição |
|---|---|
| [Pilares e Hipóteses](docs/Pilares-e-Hipoteses.md) | Filosofia de design e hipóteses a validar |
| [Personagens e Narrativa](docs/Personagens-e-Narrativa.md) | Al, Finn, O Escuro, tom e atmosfera |
| [Mecânicas](docs/Mecanicas.md) | Todas as mecânicas do jogo detalhadas |
| [Controles](docs/Controles.md) | Inputs de Al e Finn |
| [Obstáculos e Derrota](docs/Obstaculos-e-Derrota.md) | Elementos de fase e condições de derrota |
| [Estrutura das Fases](docs/Estrutura-das-Fases.md) | 10 fases e progressão de mecânicas |
| [Level Design](docs/Level-Design.md) | Princípios e construção dos mapas |
| [Arte e Som](docs/Arte-e-Som.md) | Direção visual, animações e design de áudio |
| [Prototipagem e Testes](docs/Prototipagem-e-Testes.md) | Protótipos e planejamento de playtests |
| [Stack Técnica](docs/Stack-Tecnica.md) | Tecnologias, arquitetura e CI/CD |
| [Roadmap](docs/Roadmap.md) | Cronograma e marcos de desenvolvimento |
| [Decisões Importantes](docs/Decisoes-Importantes.md) | Registro de decisões e justificativas |

---

## Como Contribuir

Este é um projeto individual com revisão em pares via squad.

1. Clone o repositório
2. Crie uma branch a partir de `develop`: `git checkout -b feature/nome-da-feature`
3. Faça suas alterações e commit: `git commit -m "feat: descrição"`
4. Abra um Pull Request para `develop`
5. Aguarde review de pelo menos 1 colega do squad

---

## Créditos

Música, sons e fontes de terceiros, usados sob a licença original de cada autor:

| Recurso | Autor | Licença |
|---|---|---|
| [Music Loop Bundle](https://tallbeard.itch.io/music-loop-bundle) | Abstraction (Tallbeard Studios) | CC0 |
| [400+ Sounds Pack](https://ci.itch.io/400-sounds-pack) | Chequered Ink | Uso livre; não revender nem redistribuir |
| [Cozy UI Sounds](https://aquumgifts.itch.io/cozy-ui-sounds) | Spaceyyy | Uso livre; não revender nem redistribuir |
| [Interface Sounds](https://kenney.nl/assets/interface-sounds) | Kenney | CC0 |
| [Input Prompts](https://kenney-assets.itch.io/input-prompts) | Kenney | CC0 |
| Pixelify Sans | Stefie Justprince | SIL OFL 1.1 |
| Press Start 2P | CodeMan38 | SIL OFL 1.1 |

---

## Licença

- Código: [MIT](LICENSE)
- Assets próprios do jogo (arte e conteúdo): CC BY-NC-SA
- Recursos de terceiros listados em [Créditos](#créditos) mantêm a licença original de cada autor

---

*PAC Extensionista VII — Engenharia de Software — Católica SC — 2026*

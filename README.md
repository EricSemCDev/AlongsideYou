# Alongside You

> *LUZ. LÓGICA. CORAGEM.*

Um jogo cooperativo onde dois jogadores controlam Al e Finn para usar luz e matemática e atravessar as paisagens da mente de Al.

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![License](https://img.shields.io/badge/Código-MIT-blue)
![Assets](https://img.shields.io/badge/Assets-CC%20BY--NC--SA-green)
![CI](https://github.com/EricSemCDev/AlongsideYou/actions/workflows/ci.yml/badge.svg)

---

## Sobre o Projeto

**Alongside You** é um puzzle cooperativo local com vista lateral 2D flat, desenvolvido como projeto de portfólio para o PAC Extensionista VII — Engenharia de Software — Católica SC.

Dois jogadores no mesmo PC controlam personagens assimétricos:

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

- **20 fases** progressivas agrupadas em 4 biomas
- **Puzzles procedurais** — cada carregamento de fase gera uma equação diferente
- **Sistema de ansiedade** — quanto mais tempo sem resolver, mais o Al deteriora
- **Obstáculos matemáticos** — Memórias de Gelo (`> X`) e Áreas de Lamento (`< X`)

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
| [Estrutura das Fases](docs/Estrutura-das-Fases.md) | 20 fases, biomas e progressão de mecânicas |
| [Arte e Som](docs/Arte-e-Som.md) | Direção visual, animações e design de áudio |
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

## Licença

- Código: [MIT](LICENSE)
- Assets: CC BY-NC-SA

---

*PAC Extensionista VII — Engenharia de Software — Católica SC — 2026*

# Stack Técnica

## Tecnologias Utilizadas

| Categoria | Ferramenta |
|---|---|
| **Engine** | Unity 2022.3 LTS |
| **Linguagem** | C# |
| **Arte** | Aseprite — pixel art |
| **Versionamento** | GitHub (repositório público) |
| **Gestão** | GitHub Projects — Kanban |
| **CI/CD** | GitHub Actions + game-ci |
| **Documentação** | Wiki do GitHub + /docs |
| **Testes** | Unity Test Framework (TDD) |
| **Distribuição** | Itch.io (WebGL) + Windows |
| **Licença (código)** | MIT |
| **Licença (assets)** | CC BY-NC-SA |

---

## Arquitetura de Software

### Princípios

- **GameManager central** — coordena estado do jogo, transições de fase e comunicação entre sistemas
- **Sistemas separados por responsabilidade** — cada script tem uma função clara
- **ScriptableObjects** para configuração de fase — parâmetros configuráveis no editor sem alterar código
- **Prefabs reutilizáveis** — bloco-dado, faísca-operador, tocha, checkpoint, obstáculos

### Sistemas Principais

| Sistema | Responsabilidade |
|---|---|
| `GameManager` | Estado global, transições de fase, condições de vitória e derrota |
| `PuzzleGenerator` | Gera equações procedurais com base no PuzzleConfig da fase |
| `AnxietySystem` | Controla os 5 estágios de ansiedade e aplica efeitos |
| `LightSystem` | Gerencia energia do Finn, decaimento global e checkpoints |
| `DistanceRope` | LineRenderer com lógica de cor baseada na distância Al-Finn |
| `CalculationStation` | Valida expressão montada, dispara reações e integra obstáculos |
| `InputManager` | Gerencia inputs do Al (teclado) e Finn (mouse) separadamente |

---

## CI/CD

O pipeline roda automaticamente em todo push para `develop` e em todo PR para `main`.

**Jobs:**
1. **Run Tests** — Unity Test Framework
2. **Build WebGL** — build para distribuição no Itch.io

**Fluxo de branches:**
```
feature/xxx → develop → main
```

- Todo desenvolvimento vai para `develop`
- Merge para `main` exige PR com review de pelo menos 1 colega do squad
- CI precisa passar antes do merge

---

## Testes

Lógica de jogo coberta por testes unitários escritos antes da implementação (TDD):

- Gerador de puzzles — valida que toda fase tem ao menos uma solução válida
- Sistema de ansiedade — valida transições de estágio e efeitos
- Avaliador de expressão — valida cálculo esquerda para direita sem prioridade
- Estação de Cálculo — valida condições `>` e `<` dos obstáculos

---

*← [Arte e Som](Arte-e-Som) | [Roadmap](Roadmap) →*

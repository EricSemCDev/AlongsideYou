# Stack Técnica

## Tecnologias Utilizadas

| Categoria | Ferramenta |
|---|---|
| **Engine** | Unity 2022.3 LTS |
| **Linguagem** | C# |
| **Arte** | Aseprite — pixel art |
| **Versionamento** | GitHub (repositório público) |
| **Gestão** | GitHub Projects — metodologia Kanban |
| **CI/CD** | GitHub Actions + game-ci |
| **Documentação** | Wiki do GitHub |
| **Testes** | Unity Test Framework (TDD na lógica do jogo) |
| **Distribuição** | Itch.io (WebGL) + executável Windows |
| **Licença (código)** | MIT |
| **Licença (assets)** | CC BY-NC-SA |

---

## Arquitetura

### Princípios

- **ScriptableObjects** para configuração de fases — parâmetros do puzzle, dificuldade, operadores permitidos e range de valores configuráveis no editor sem alterar código
- **Prefabs reutilizáveis** para elementos de fase — bloco-dado, faísca-operador, tocha, checkpoint, obstáculos
- **Separação de responsabilidades** — cada sistema tem script próprio, sem acoplamento direto

### Sistemas Principais

| Sistema | Responsabilidade |
|---|---|
| `PuzzleGenerator` | Gera equações proceduralmente com base no PuzzleConfig da fase |
| `AnxietySystem` | Controla os estágios de ansiedade do Al e aplica os efeitos |
| `LightSystem` | Gerencia energia do Finn, decaimento global e checkpoints |
| `DistanceRope` | LineRenderer que conecta Al e Finn, muda de cor por distância |
| `CalculationStation` | Valida expressão montada e dispara reações no mapa |

---

## CI/CD

O pipeline roda automaticamente em todo push para `develop` e em todo PR para `main`.

### Jobs

1. **Run Tests** — executa testes unitários via Unity Test Framework
2. **Build WebGL** — gera a build para distribuição no Itch.io

### Fluxo de branches

```
feature/xxx → develop → main
```

- Todo desenvolvimento vai para `develop`
- Merge para `main` exige PR com review de pelo menos 1 colega do squad
- CI precisa passar antes do merge

---

## Testes

A lógica de jogo é coberta por testes unitários escritos antes da implementação (TDD):

- Gerador de puzzles — valida que toda fase gerada tem pelo menos uma solução válida
- Sistema de ansiedade — valida transições de estágio e efeitos aplicados
- Avaliador de expressão — valida cálculo da esquerda para direita sem prioridade de operações
- Validação da Estação de Cálculo — valida condições `>` e `<` dos obstáculos

---

*← [Arte e Som](Arte-e-Som) | [Roadmap](Roadmap) →*

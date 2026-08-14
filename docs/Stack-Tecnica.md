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
| **Testes** | Unity Test Framework (uso pontual, sem TDD obrigatório) |
| **Distribuição** | Itch.io (WebGL) + Windows |
| **Licença (código)** | MIT |
| **Licença (assets)** | CC BY-NC-SA |

---

## Arquitetura de Software

### Princípios

- **GameManager central** — coordena estado do jogo, transições de fase e comunicação entre sistemas
- **GameManager em duas camadas** — `GameStateMachine` (classe C# pura, sem dependência da engine) e `GameManager` (MonoBehaviour singleton que a instancia e expõe às demais cenas)
- **Sistemas separados por responsabilidade** — cada script tem uma função clara
- **ScriptableObjects** para configuração de fase — parâmetros configuráveis no editor sem alterar código
- **Prefabs reutilizáveis** — bloco-dado, faísca-operador, tocha, checkpoint, obstáculos

### Sistemas e Interações

Os sistemas se comunicam por eventos — sem acoplamento direto entre scripts.

| Sistema | Responsabilidade | Notifica |
|---|---|---|
| `GameManager` | Estado global, transições de fase, vitória e derrota | Todos os sistemas ao mudar de fase |
| `PuzzleGenerator` | Gera equações por geração reversa a partir do resultado | `CalculationStation` com a equação gerada |
| `AnxietySystem` | Controla os 5 estágios de ansiedade e aplica efeitos progressivos | `LightSystem` (diminuir raio no Estágio 3) e `GameManager` (Estágio 5 atingido) |
| `LightSystem` | Gerencia energia do Finn, decaimento global e checkpoints | `AnxietySystem` quando Finn ilumina Al no Estágio 5 (resgate) |
| `DistanceRope` | LineRenderer com lógica de cor por distância | `AnxietySystem` (distância crítica) e `GameManager` (separação prolongada = derrota) |
| `CalculationStation` | Valida expressão montada e dispara reações no mapa | `AnxietySystem` (equação correta alivia estágio) e `GameManager` (resultado válido) |
| `InputManager` | Gerencia inputs de Al (teclado/gamepad) e Finn (mouse/gamepad), bloqueando input fora do estado Playing | Al e Finn a cada frame, zerados quando pausado ou em menu |
| `FinnTargetSystem` | Calcula o ponto-alvo que o Finn persegue, unificando mouse (clique + direção) e gamepad (analógico direito) com `Mathf.SmoothDamp` | Movimento do Finn a cada frame |

---

## PuzzleGenerator — Algoritmo de Geração Reversa

O gerador parte do resultado e constrói a equação de trás para frente, garantindo solução em 100% dos casos:

1. Sorteia resultado alvo dentro do range configurado para a fase
2. Aplica operadores inversos a partir do resultado para encontrar os valores dos blocos
3. Gera as demais faces de cada bloco com valores aleatórios dentro do range permitido
4. Seleciona faíscas-operador necessárias para a solução e adiciona extras como distratores

**Repositório do algoritmo:** [github.com/EricSemCDev/Equation-Generator](https://github.com/EricSemCDev/Equation-Generator)

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

*← [Arte e Som](Arte-e-Som) | [Prototipagem e Testes](Prototipagem-e-Testes) →*

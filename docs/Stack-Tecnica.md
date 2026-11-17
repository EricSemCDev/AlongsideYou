# Stack Técnica

## Tecnologias Utilizadas

| Categoria | Ferramenta |
|---|---|
| **Engine** | Unity 2022.3 LTS, URP 2D, Input System, TextMeshPro |
| **Linguagem** | C# |
| **Arte** | Aseprite — pixel art |
| **Versionamento** | Git + GitHub (issues, milestones e pull requests) |
| **CI/CD** | GitHub Actions + game-ci |
| **Documentação** | GDD, Wiki do GitHub e `/docs` |
| **Testes** | Playtests (testes automatizados removidos com aval dos professores) |
| **Distribuição** | Itch.io (WebGL) + Windows |
| **Licença (código)** | MIT |
| **Licença (assets próprios)** | CC BY-NC-SA |

---

## Arquitetura de Software

### Princípios

- **Gerentes persistentes** (sobrevivem à troca de cena): `GameManager` (estados do jogo), `InputManager` (controles), `PhaseProgressionManager` (sequência de fases), `SceneTransitionManager` (fade entre cenas), menu de pausa e `MusicPlayer`
- **Comunicação por eventos** — a estação avisa quando a conta é validada, a ansiedade avisa quando muda de estágio, e cada sistema interessado reage sozinho
- **ScriptableObjects** para configurar sem código — `PuzzleConfig` (puzzle da fase), `AnxietyConfig` (ansiedade da fase), `PhaseSequence` (ordem das fases) e `FeedbackSounds` (sons)
- **Prefabs reutilizáveis** — Al, Finn, bloco, faísca, espaços, tochas, estação, espinhos, Fagulha, gerador e o kit de fase (`FaseEssencials`)
- **Feito por código quando possível** — caixa de diálogo, partículas, neblina, sombras e efeitos são criados em tempo de execução

### Estados do Jogo

O `GameManager` guarda o estado atual (`Menu`, `Playing`, `Paused`, `GameOver`, `Complete`) e só aceita trocas permitidas por uma tabela (por exemplo `Playing → Paused`, `Paused → Playing`, `Playing → Complete`). A pausa zera o tempo do jogo. A troca para `Playing` só acontece depois que a cena nova termina de carregar.

### Sistemas e Interações

| Sistema | Responsabilidade | Conversa com |
|---|---|---|
| `GameManager` | Estado do jogo e transições válidas | Todos consultam se o jogo está rodando |
| `InputManager` | Lê teclado, mouse e controles; modos Solo e Co-op; troca de personagem | Al, Finn e menus; suspende a entrada durante o tutorial |
| `PhaseProgressionManager` + `PhaseSequence` | Fase ativa, próxima fase, reiniciar, voltar ao menu | `SceneTransitionManager`, `PhaseProgressSave`, `MainMenuUI` (créditos no fim) |
| `PhaseCompletionManager` + `PhaseResultScreen` | Cronômetro, estrelas e tela de resultado | `PhaseProgressionManager` |
| `PuzzleGenerator` + `PuzzleConfig` | Gera a equação e aplica nos blocos, faíscas e no alvo | `CalculationStationDisplay` |
| `CalculationStationDisplay` | Lê espaços e tochas, calcula, confirma e conta tentativas | Barreira de espinhos, efeitos e sons (evento de validação) |
| `ThornBarrier` | Abre de vez quando a conta está certa | Escuta a estação |
| `AnxietySystem` | Sobe e desce estágios pela luz | Efeitos de cor, velocidade e luz do Al; `AnxietyDefeatTrigger` |
| `AmbientTorch` | Tocha que queima, apaga e é reacendida pelo Finn | `AnxietySystem` (pelo raio da luz) |
| `AlBlockCarrier` + `DadoBlock` + `NumericSlot` | Pegar, girar, largar e encaixar blocos | Estação |
| `FinnMovement` + `FinnOperatorCollector` + `OperatorMenuController` | Movimento do Finn, estoque de operadores e roda | Tochas de operador |
| Tutorial (`TutorialSequencePlayer`, `TutorialDialogueUI`, `TutorialCameraFocus`, ícones) | Falas, câmera e ícones de botão | `InputManager` e cronômetro (pausa os dois) |
| Juice (`GameJuice`, `CameraShake`, `FeedbackAudio`) | Faíscas, tremor e sons | Chamado por uma linha onde o evento acontece |
| Menus e `MusicPlayer` | Menu principal, pausa, créditos e música | `GameManager` e `PhaseProgressionManager` |

---

## PuzzleGenerator — Como a Equação Nasce

O gerador é o algoritmo do [Equation-Generator](https://github.com/EricSemCDev/Equation-Generator) portado para dentro da Unity. Ele roda sozinho quando a fase carrega:

1. Sorteia os operadores da conta a partir de um estoque com operadores fáceis (`+`, `−`) e difíceis (`×`, `÷`), conforme a dificuldade
2. Sorteia um número para cada espaço (compostos recebem de 10 a 99) e calcula a conta da esquerda para a direita
3. Descarta a tentativa se houver divisão com resto, resultado acima do máximo ou negativo onde não pode; repete até 1.000 vezes, trocando os operadores até 30 vezes
4. Com uma conta válida, o resultado vira o alvo. Cada dígito usado é plantado num bloco, as outras faces recebem números aleatórios e tudo é embaralhado; as faíscas recebem os símbolos do estoque (incluindo iscas)
5. Se nada der certo, usa uma equação de segurança só com somas e avisa no Console

Como o alvo sempre vem de uma conta que de fato existe, **toda fase tem solução** — o mesmo efeito da geração reversa sugerida pelo professor. O gerador trabalha numa cópia do `PuzzleConfig` em memória, então o asset nunca é alterado durante o jogo.

---

## CI/CD

O workflow `main.yml` (GitHub Actions + game-ci) roda em todo push e pull request e gera o **build WebGL**, publicado como artefato.

**Fluxo de branches:**
```
feature/xxx → develop → main
```

- Cada mudança entra por pull request
- `main` é protegida: merge só por pull request revisado

---

*← [Arte e Som](Arte-e-Som) | [Prototipagem e Testes](Prototipagem-e-Testes) →*

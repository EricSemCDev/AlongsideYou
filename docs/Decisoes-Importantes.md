# Decisões Importantes

Registro de decisões relevantes tomadas durante o projeto, com data e justificativa.

---

## 06/03/2026

### Nome do jogo alterado para Alongside You
Nome anterior (Don't Cry Al) era fraco para portfólio. Alongside You captura parceria e superação emocional de forma poética e memorável.

### Personagem Bobby renomeado para Finn
Al + Finn = **Final** — reforça narrativamente que a parceria entre os dois personagens é o que leva Al ao fim da jornada. Nome anterior não tinha identidade.

### Modo luz forte removido
A mecânica não resolvia um problema de gameplay que o botão de interação já não cobrisse. Removida para simplificar o conjunto de ações do Jogador 2 sem perda de profundidade.

### Blocos numéricos redesenhados como blocos-dado com faces giráveis
Valores fixos tornariam cada puzzle uma busca pela combinação correta, eliminando criatividade. Faces giráveis com intervalos de valor permitem múltiplas soluções válidas e decisão ativa durante o trajeto.

### Faíscas-operador adicionadas como mecânica do Finn
Finn estava passivo — só iluminava e seguia. Coletar operadores matemáticos espalhados pelo mapa torna o Jogador 2 agente ativo do puzzle, não apenas suporte de navegação.

### Escuridão definida como estática
Escuridão dinâmica adicionaria complexidade de implementação sem ganho narrativo. A ansiedade do Al não persegue — ela ocupa espaço e espera. Escuridão estática com decaimento de luz global entrega a mesma tensão com menos risco técnico.

---

## 10/03/2026

### Obstáculo 'vento' redesenhado como Áreas de Lamento
Vento não tinha conexão narrativa com o mundo interno de Al. Áreas úmidas com goteiras representam memórias dolorosas — assuntos que Al evita. A metáfora fecha em todos os níveis: narrativo, visual e mecânico.

### Plantas fotossensíveis redesenhadas como Memórias de Gelo
Plantas não tinham justificativa dentro da mente de Al. Estruturas de gelo que derretem com a luz do Finn representam memórias boas que Al esqueceu. Só Al pode atravessá-las — Finn não participou dessas memórias. Reforça a assimetria dos personagens com camada narrativa.

### Raio de distância Al-Finn comunicado por corda de luz em vez de indicador numérico
Número de metros na UI quebraria a imersão do mundo interno de Al. Corda de luz entre os personagens comunica a mesma informação de forma diegética, narrativamente coerente e legível para ambos os jogadores simultaneamente.

### Puzzle gerado proceduralmente a cada carregamento de fase
Puzzles fixos permitiriam memorização da solução, eliminando o raciocínio matemático em repetições. A geração procedural garante que cada partida exige novo raciocínio.

### Sistema de ansiedade progressiva em estágios adicionado
Professor apontou falta de fator de urgência. Sistema de ansiedade cria pressão narrativamente coerente sem inimigos externos — a pressão vem de dentro do personagem, não de fora.

### Obstáculos integrados ao sistema de puzzle via condições > e <
Conecta a matemática ao mundo do jogo. O resultado da equação agora tem impacto direto nos obstáculos do mapa, tornando cada puzzle contextualmente relevante para a navegação.

### Resolver equação alivia estágio de ansiedade
Reforça que a matemática é a ferramenta de superação — cada puzzle resolvido dá alívio narrativo e mecânico, criando um loop de recompensa coerente com a narrativa.

---

## 15/03/2026

### Migração para Unity 2022.3 LTS
Unity 6000.3.11f1 não tinha imagem Docker disponível no game-ci, impossibilitando CI/CD. A versão 2022.3 LTS tem suporte completo, estável e imagens Docker publicadas. Para um platformer 2D, não há perda de funcionalidade relevante.

---

*← [Roadmap](Roadmap) | [Home](Home) →*

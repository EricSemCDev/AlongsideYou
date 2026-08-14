# Roadmap

## Cronograma

| Período | Marco | Entregas |
|---|---|---|
| **Port. I — Mar–Mai 2026** | RFC + Documentação | GDD finalizado, RFC completa, validação de demanda, repositório configurado |
| **Férias — Jun–Jul 2026** | Fundação Técnica | Unity configurado, CI/CD, protótipos de movimentação e Estação de Cálculo |
| **Agosto 2026** | Alpha Core | Al e Finn funcionais, Estação de Cálculo, sistema de ansiedade com Estágio 5 |
| **Setembro 2026** | Alpha Completo | Bloco 1 (fases 1–4), gerador procedural, sistema de estrelas, playtests iniciais |
| **Outubro 2026** | Beta | Fases 5–15, obstáculos, fases secretas, skins, playtests externos |
| **Novembro 2026** | Gold + Entrega | Fase 16, arte final, som, menus, build Itch.io, playtests finais |

---

## Escopo

### O jogo inclui

- 16 fases principais — 4 por bloco
- 4 fases secretas — uma por bioma, desbloqueadas com 3 estrelas em todas as fases do bloco
- Sistema de estrelas — 1 a 3 por fase baseado no tempo de conclusão
- Skins desbloqueáveis — roupa alternativa do Al e cor da chama do Finn
- 2 personagens com controles assimétricos — Al (teclado) e Finn (mouse ou gamepad)
- Sistema de puzzle procedural com geração de equações únicas por sessão
- Sistema de ansiedade progressiva em 5 estágios como dificuldade crescente
- Build WebGL gratuita no Itch.io

### O jogo não inclui

- Sistema de economia interna — sem moeda, XP ou itens consumíveis. Distribuição gratuita via Itch.io. Monetização futura via doação voluntária.
- Multiplayer online — cooperativo local é o escopo
- Cálculos com fração, potenciação ou prioridade de operações
- Sistema de save — cada sessão começa do início
- Escuridão dinâmica com comportamento ativo

---

## Limitações Conhecidas

- Multiplayer online
- Sistema de save
- Escuridão dinâmica com comportamento ativo
- Cálculos com fração, potenciação ou prioridade de operações
- Suporte a mais de 2 jogadores
- Sistema de economia interna — decisão intencional de design
- Localização — o jogo será desenvolvido em português

---

## Riscos do Projeto

| Risco | Impacto | Mitigação |
|---|---|---|
| Arte pixel art consumir mais tempo | Alto | Placeholders durante todo o desenvolvimento, arte final nas últimas semanas |
| Ergonomia do co-op teclado+mouse | Médio | Suporte a gamepad para Finn. Playtests com diferentes configurações de assento. |
| Balanceamento do sistema de ansiedade | Médio | Parâmetros via ScriptableObject para ajuste rápido. Playtest frequente. |
| Tempos-alvo para estrelas difíceis de calibrar | Médio | Coletar dados de tempo nos playtests iniciais antes de definir os targets finais |
| Cooperativo local dificultar playtests solo | Baixo | Modo debug com Finn controlado por IA simples para testes internos |

---

*← [Prototipagem e Testes](Prototipagem-e-Testes) | [Decisões Importantes](Decisoes-Importantes) →*

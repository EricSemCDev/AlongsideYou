# Roadmap

## Cronograma

| Período | Marco | Entregas |
|---|---|---|
| **Port. I — Mar–Mai 2026** | RFC + Documentação | GDD v7, RFC completa, validação de demanda, repositório configurado |
| **Férias — Jun–Jul 2026** | Fundação Técnica | Unity configurado, CI/CD funcionando, protótipo de movimentação |
| **Agosto 2026** | Alpha Core | Al e Finn funcionais, Estação de Cálculo, sistema de ansiedade |
| **Setembro 2026** | Alpha Completo | Bloco 1 (fases 1–5), gerador procedural, playtests iniciais |
| **Outubro 2026** | Beta | Fases 6–19, obstáculos matemáticos, playtests externos |
| **Novembro 2026** | Gold + Entrega | Fase 20, arte final, som, menus, build Itch.io |

---

## Fases de Desenvolvimento

### Alpha
Versão funcional mas sem polimento. Foco em validar mecânicas centrais.

- Movimentação do Al e Finn funcionando
- Estação de Cálculo básica — A op B
- Sistema de ansiedade em estágios
- Regra do escuro e raio de distância
- Bloco 1 completo — 5 fases jogáveis
- Build WebGL no Itch.io

### Beta
Feature complete. Todas as mecânicas implementadas, foco em ajustes.

- Blocos-dado de 6 faces e expressões A op B op C
- Áreas de Lamento e Memórias de Gelo
- Finn passando por aberturas estreitas
- Fases 6 a 19 implementadas
- Playtests com público externo

### Gold
Versão final. Polimento completo para entrega.

- Arte pixel art substituindo placeholders
- Som e trilha final com progressão por estágio
- Fase 20 implementada
- Menus, tela de game over e tela de conclusão
- Build final no Itch.io

---

## Limitações Conhecidas

- Multiplayer online — cooperativo local é o escopo
- Sistema de save — cada sessão começa do início
- Escuridão dinâmica com comportamento ativo
- Cálculos com fração, potenciação ou prioridade de operações
- Suporte a mais de 2 jogadores
- Localização — desenvolvido em português

---

## Riscos do Projeto

| Risco | Impacto | Mitigação |
|---|---|---|
| Arte pixel art consumir mais tempo | Alto | Placeholders durante todo o desenvolvimento, arte final nas últimas semanas |
| Balanceamento da ansiedade difícil de calibrar | Médio | Parâmetros via ScriptableObject. Playtest frequente. |
| Gerador procedural sem soluções com certos parâmetros | Médio | Fallback para equação hardcoded após N tentativas |
| Cooperativo dificultar playtests solo | Baixo | Modo debug com Finn controlado por IA simples |

---

*← [Stack Técnica](Stack-Tecnica) | [Decisões Importantes](Decisoes-Importantes) →*

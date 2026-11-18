# Roadmap

## Cronograma (realizado)

| Período | Marco | Entregas |
|---|---|---|
| **Mar–Mai 2026** | RFC e documentação | GDD, RFC, validação de demanda, repositório configurado |
| **Jun–Jul 2026** | Fundação técnica | Unity configurado, CI/CD, primeiros protótipos |
| **Agosto 2026** | Alpha Core (início) | Al com colisão, Finn com ponto-alvo, blocos-dado com giro, `PuzzleConfig` |
| **Setembro 2026** | Alpha Core | Estação de Cálculo, roda de operadores, ansiedade e luz, gerador de equações, vitória e derrota, menus |
| **Outubro 2026** | Beta jogável | Fases em sequência, arte e iluminação, Prólogo com tutorial, suporte a controle, playtest com formulário |
| **Novembro 2026** | Gold e entrega | Estrelas e tela de resultado, fases 6 a 10, sons e música, créditos, builds WebGL e Windows |

---

## Escopo

### O jogo inclui

- Prólogo com tutorial guiado (diálogo do Finn, câmera e ícones de botão que se adaptam ao teclado ou ao controle)
- 10 fases no bioma A Névoa, com dificuldade crescente
- Puzzle procedural: uma equação nova a cada vez que a fase carrega
- Espaços numéricos simples e compostos (dois dígitos)
- Roda de operadores do Finn com estoque de faíscas
- Sistema de ansiedade em 5 estágios, dirigido pela luz
- Tochas de ambiente que apagam e são reacendidas pelo Finn
- Estrelas por tempo, com tela de resultado e melhor nota salva
- Modos Solo e Co-op local, com teclado, mouse e controles Xbox, PlayStation e Switch
- Menus principal, de pausa e de resultado, navegáveis por controle
- Música de fundo e efeitos sonoros por evento
- Créditos com rolagem, que também rodam ao terminar a última fase
- Build WebGL gratuita no Itch.io e build para Windows

### O jogo não inclui

- Fases secretas e skins desbloqueáveis
- Os biomas O Labirinto, O Abismo e O Centro
- Obstáculos de condição (`>`, `<`, paridade, sinal) e placas de pressão nas fases
- Corda de luz entre Al e Finn, energia do Finn e derrota por distância
- Resgate do Al pelo Finn no Estágio 5 de ansiedade
- Tela de seleção de fases
- Opções de configuração (volume, tremor de câmera) — o painel existe no menu, mas está vazio
- Economia interna (moeda, XP, itens) — distribuição gratuita
- Multiplayer online
- Frações, potências ou prioridade de operadores

---

## Limitações Conhecidas

- Um único bioma (A Névoa) e 10 fases
- Sem opções de configuração (volume e tremor de câmera)
- Sem seleção de fases nem "continuar": ao fechar o jogo, a próxima partida começa do Prólogo (só a melhor nota de cada fase fica salva)
- Sem localização: o jogo está em português
- Sem multiplayer online e sem suporte a mais de dois jogadores
- Sem frações, potências ou prioridade de operadores
- Obstáculos de condição (`>`, `<`, paridade, sinal), placas de pressão e a alavanca de confirmação existem no código, mas não são usados em nenhuma fase
- Espaço composto: os dois blocos ainda usam um visual simples

---

## Riscos do Projeto

| Risco | Impacto | Mitigação | Situação |
|---|---|---|---|
| Arte em pixel art consumir tempo demais | Alto | Placeholders no início; um único bioma reaproveitado nas 10 fases | Mitigado |
| Ergonomia do co-op teclado + mouse | Médio | Suporte a controle e modo Solo | Mitigado |
| Balanceamento da ansiedade | Médio | `AnxietyConfig` por fase, ajustável no Editor | Ajustado com playtest |
| Tempos de estrela difíceis de calibrar | Médio | Tempos por fase no `PhaseManager` | Ajustado por fase |
| Gerador sem solução ou travando | Alto | Limite de tentativas, troca de operadores e equação de segurança | Mitigado |
| Prazo curto para o escopo original (20 fases) | Alto | Corte para 10 fases e remoção de sistemas não essenciais | Mitigado |
| Co-op com dois controles pouco testado | Baixo | Testar com dois controles antes da entrega | Em aberto |

---

*← [Prototipagem e Testes](Prototipagem-e-Testes) | [Decisões Importantes](Decisoes-Importantes) →*

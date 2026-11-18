# Arte e Som

## Direção de Arte

- Pixel art flat, feita no **Aseprite**: sprites próprios do Al, do Finn, árvores, tochas, blocos, espaços, faíscas, espinhos e Fagulha
- Profundidade por **Y-sorting** (quem está mais embaixo na tela fica na frente) e sombras ovais sob os personagens
- Objetos à frente ficam translúcidos quando o jogador passa atrás deles
- Iluminação 2D do URP: luz do Al, luz do Finn e tochas, com tremulação de fogo
- Neblina em camadas com parallax e pontinhos de luz flutuando no vazio, criados por código
- Rastro de chamas e de "fantasmas" atrás do Finn quando ele se move rápido
- Contorno dos objetos interagíveis revelado pela proximidade do jogador
- Efeitos de "suco": faíscas ao acertar, ao acender a tocha, ao abrir a barreira e ao pegar a Fagulha; tremor curto de câmera ao errar e ao abrir a barreira

---

## Referências Visuais

| Referência | Inspiração |
|---|---|
| South Park: The Stick of Truth | Visual flat, personagens recortados no cenário |
| Hollow Knight | Atmosfera escura, paleta fria, emoção sem texto |
| Celeste | Game feel e ansiedade sem punição |
| Moonleap | Puzzle cooperativo com papéis assimétricos |
| The Binding of Isaac | Pixel art expressivo com personagens pequenos e legíveis |
| Rotom (Pokémon) | Character design do Finn: chama com rosto expressivo |

---

## Animações

| Animação | Objeto | Descrição |
|---|---|---|
| Idle e andar | Al | Parado e caminhando; o sprite vira conforme a direção |
| Idle e voar | Finn | Chama oscilando parada e em movimento, com rastro de chamas |
| Escurecer por ansiedade | Al | Cor vai do normal ao preto entre os estágios 1 e 5 (por código) |
| Acender e apagar | Tocha de ambiente | A tocha acende, queima e apaga; a luz acompanha |
| Mais, Menos, Vezes, Dividido | Tocha de operador | Animação do operador acendendo, depois o símbolo flutuando |
| Vazio e preenchido | Espaço numérico (simples e composto) | Muda o desenho quando o bloco encaixa |
| Encaixado | Bloco-dado | O cubo vira um quadrado ao entrar no espaço |
| Murchar | Espinhos | Os espinhos murcham juntos quando a conta está certa |
| Brilho | Fagulha de Luz | Brilho em loop, flutuando |
| Balanço | Árvores | Movimento suave de vento |
| Interface | Telas | Estrelas enchendo com pulinho, botões aparecendo com fade, cartão do nome da fase, roda de operadores com "pop" |

---

## Áudio

Uma única música em loop toca do menu até o fim do jogo, sem reiniciar entre as fases, com fade de entrada. Os efeitos ficam num asset só (`FeedbackSounds`), com um clipe e um volume por evento; evento sem clipe fica em silêncio.

| Grupo | Eventos com som |
|---|---|
| Música | Loop contínuo (Music Loop Bundle, Abstraction) |
| Menus | Passar o cursor, clicar, voltar, abrir e fechar a pausa |
| Tutorial | Digitação das letras (com tom variando), abrir a caixa, avançar a fala, foco de câmera |
| Bloco | Pegar, largar, girar a face, encaixar no espaço |
| Finn e operadores | Coletar faísca, abrir a roda, passar pelos operadores, erro (operador indisponível), acender a tocha da estação |
| Conta | Acertou (também cobre a barreira abrindo) e errou |
| Fim da fase | Coletar a Fagulha (vitória) e cada estrela que enche na tela de resultado |

> Ficaram sem som de propósito: tochas de ambiente apagando e reacendendo (várias ao mesmo tempo poluiriam o som), fechar a caixa de diálogo, pular o tutorial e tentativas esgotadas.

Fontes de música, sons e fontes tipográficas em [Créditos](Creditos).

---

*← [Estrutura das Fases](Estrutura-das-Fases) | [Stack Técnica](Stack-Tecnica) →*

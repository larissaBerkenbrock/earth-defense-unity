# Earth Defense

Jogo 2D de ação desenvolvido em **Unity** com **C#**, como projeto académico do curso de Desenvolvimento Web e Multimédia — Instituto Politécnico de Castelo Branco (ano letivo 2025/2026).

Projeto individual: programação, configuração das cenas, animações e integração de sprites e áudio.

## Como se joga

O jogador enfrenta vagas de inimigos (*Scouts*) que surgem dos dois lados do ecrã e tem de recolher núcleos de energia para avançar. O jogo tem **3 níveis**, cada um mais difícil que o anterior: os inimigos aparecem mais depressa, são mais rápidos, têm mais vida e causam mais dano.

Para concluir cada nível é preciso **derrotar 10 Scouts** e **recolher 5 núcleos de energia**.

### Controlos

| Tecla | Ação |
|---|---|
| ← → / A D | Mover |
| Espaço | Disparar |
| Shift esquerdo | Dash |

## Funcionalidades

- Menu principal, menu de pausa, ecrã de vitória e ecrã de game over
- Sistema de níveis configurável (velocidade de spawn, vida e dano dos inimigos)
- Arma diferente em cada nível: **Pistola Básica → Rifle de Plasma → Raio Laser**
- Núcleos de energia que recuperam vida; a cada 10 núcleos o jogador ganha um bónus temporário de velocidade ("adrenalina")
- Barra de vida e interface com o progresso do nível
- Animações do jogador (parado, a andar, morte), efeitos sonoros e música

## Tecnologias

- Unity 6 (6000.3) — Universal Render Pipeline 2D
- C#
- TextMesh Pro
- Animator (máquinas de estados para animações)

## Estrutura dos scripts
Assets/scripts/
├── Player/ # movimento, disparo, dash e vida do jogador
├── Inimigos/ # movimento e vida dos Scouts
├── SpawnManager.cs # níveis, aparecimento de inimigos e progressão
├── WeaponManager.cs # armas de cada nível
├── EnergyManager.cs # núcleos de energia e bónus
├── VictoryManager.cs, GameOverManager.cs, PauseMenu.cs, MainMenu.cs


## Como abrir o projeto

1. Clonar ou descarregar este repositório
2. Abrir a pasta no **Unity Hub** com a versão **6000.3.10f1** (ou compatível)
3. Abrir a cena `Assets/Scenes/MainMenu.unity` e carregar em **Play**

## Autora
Larissa Berkenbrock

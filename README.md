# RogueSweeper
<img src="readme_imgs/Title.png"/>

This is RogueSweeper, an extension/re-make of my [JavaSweeper Infinite](https://github.com/SmileyFace799/javasweeper_infinite) Java project, which again is a discontinued & unfinished re-make of my `Minesweeper Infinite` Python project (source code lost), which is quite simply Minesweeper, but progressively generated & infinite. However, this version aims to not just make infinite minesweeper, but also to expand on it & turn it into a roguelite.

<a name="table-of-contents"></a>
<br/><h1><img alt="Table of contents" src="readme_imgs/TableOfContents.png"/></h1>

1. [How to play](#how-to-play)
    1. [Tile reference](#tile-reference)
        1. [Regular number tiles](#regular-number-tiles)
        2. [Defused number tiles](#defused-number-tiles)
        3. [Bomb tile](#bomb-tile)
        4. [Life tile](#life-tile)
        5. [Bad chance reduction tile](#bad-chance-reduction-tile)
        6. [Power-up tiles](#power-up-tiles)
        7. [Cross number tiles (Currently unused)](#cross-number-tiles-currently-unused)
        8. [Plus number tiles (Currently unused)](#plus-number-tiles-currently-unused)
        9. [Diamond number tiles (Currently unused)](#diamond-number-tiles-currently-unused)
    2. [Power-up reference](#power-up-reference)
        1. [Small solver](#small-solver)
        2. [Medium solver](#medium-solver)
        3. [Large solver](#large-solver)
        4. [Defuser](#defuser)
2. [Video demo](#video-demo)
2. [Development info](#development-info)
    1. [Deadline](#deadline)
    2. [Goals of this project](#goals-of-this-project)
    3. [Features](#features)

<a name="how-to-play"></a>
<br/><h1><img alt="How to play" src="readme_imgs/HowToPlay.png"/></h1>
All the standard rules of minesweeper apply in this game, you can click on squares to open them, you wanna open as many squares as possible, while avoiding the bad squares. You can right click to place flags, and if you've found/marked every bad square around a number, you can middle click to instantly reveal the remaining squares around that number. However, there are also some new tiles you might encounter, and a full reference on every tile you can encounter is listed below.

## Tile Reference
### Regular number tiles:
<img alt="Regular number tiles" src="readme_imgs/DefaultNumbers.png"/>

- **Type:** Number
- **Severity:** Neutral
- **Spawns from:** Natural generation
- **Effect:** Counts how many bad squares exist within its coverage
- **Coverage:**<br/><img src="readme_imgs/DefaultNumbersCoverage.png"/>

### Defused number tiles:
<img alt="Defused number tiles" src="readme_imgs/DefusedBombNumbers.png"/>

- **Type:** Number
- **Severity:** Bad
- **Spawns from:** [Defuser power-up](#defuser)
- **Effect:** Counts how many bad squares exist within its coverage (Does **not** count itself)
- **Coverage:**<br/><img src="readme_imgs/DefusedBombNumbersCoverage.png"/>

### Bomb tile:
<img alt="Bomb tile" src="readme_imgs/BombTile.png"/>

- **Type:** Special
- **Severity:** Bad
- **Spawns from:** Natural generation
- **Effect:** Lose a life

### Life tile:
<img alt="Life tile" src="readme_imgs/LifeTile.png"/>

- **Type:** Special
- **Severity:** Good
- **Spawns from:** Natural generation
- **Effect:** Gain a life

### Bad chance reduction tile:
<img alt="Bad chance reduction tile" src="readme_imgs/BadChanceReductionTile.png"/>

- **Type:** Special
- **Severity:** Good
- **Spawns from:** Natural generation
- **Effect:** Reduce the chance of generating bad squares in the future (Does **not** affect squares that are already generated)

### Power-up tiles:
<img alt="Power-up tiles" src="readme_imgs/PowerUpTiles.png"/>

- **Type:** Special
- **Severity:** Good
- **Spawns from:** Natural generation
- **Effect:** Gain a corresponding [power-up](#power-up-reference) (see image)

### Cross number tiles (Currently unused):
<img alt="Cross number tiles" src="readme_imgs/CrossNumbers.png"/>

- **Type:** Number
- **Severity:** Neutral
- **Spawns from:** Nothing
- **Effect:** Counts how many bad squares exist within its coverage
- **Coverage:**<br/><img src="readme_imgs/CrossNumbersCoverage.png"/>

### Plus number tiles (Currently unused):
<img alt="Plus number tiles" src="readme_imgs/PlusNumbers.png"/>

- **Type:** Number
- **Severity:** Neutral
- **Spawns from:** Nothing
- **Effect:** Counts how many bad squares exist within its coverage
- **Coverage:**<br/><img src="readme_imgs/PlusNumbersCoverage.png"/>

### Diamond number tiles (Currently unused):
<img alt="Diamond number tiles" src="readme_imgs/DiamondNumbers.png"/>

- **Type:** Number
- **Severity:** Neutral
- **Spawns from:** Nothing
- **Effect:** Counts how many bad squares exist within its coverage
- **Coverage:**<br/><img src="readme_imgs/DiamondNumbersCoverage.png"/>


## Power-Up Reference
Power-ups are a new addition to this game, to spice up the gameplay & to add some more strategy. These are active-use effects that can be selected & used on a single tile. To use one, simply select the power-up to use, then left click on a tile on the board to use it there. Right clicking with a power-up selected deselects it.

### Small solver:
<img alt="Small solver" src="readme_imgs/SmallSolver.png"/>

- **Area of effect:** 1x1
- **Effect:** For every affected tile, places a flag on it if it's a bad square, otherwise opens it. Does nothing on already opened tiles

### Medium solver:
<img alt="Medium solver" src="readme_imgs/MediumSolver.png"/>

- **Area of effect:** 3x3
- **Effect:** For every affected tile, places a flag on it if it's a bad square, otherwise opens it. Does nothing on already opened tiles

### Large solver:
<img alt="Large solver" src="readme_imgs/LargeSolver.png"/>

- **Area of effect:** 5x5
- **Effect:** For every affected tile, places a flag on it if it's a bad square, otherwise opens it. Does nothing on already opened tiles

### Defuser:
<img alt="Defuser" src="readme_imgs/Defuser.png"/>

- **Area of effect:** 1x1
- **Effect:** If the target tile is a bad square, nullify its effect & place a [defused number tile](#defused-number-tiles) there instead


<a name="video-demo"></a>
<br/><h1><img alt="Video Demo" src="readme_imgs/VideoDemo.png"/></h1>
<video src="demo.mp4"></video>

<a name="development-info"></a>
<br/><h1><img alt="Development Info" src="readme_imgs/DevelopmentInfo.png"/></h1>

## Deadline
The deadline for the MVP of the project is 6/1/2025. Anything outside the MVP will not be prioritized until after this deadline. Beyond the deadline, this will probably become more of a "passion project", where development will happen whenever I feel like it, without any time constraints.

## Goals of this project
The "end goal" of this project is simply infinite Minesweeper, but expanded with some unique features to adapt it into a roguelite. There's a few smaller goals along the way (listed below), which will be checked off as the project develops. More specific goals will be tracked using an issue board after MVP has been reached.

**List of goals (within deadline):**
1. Create a runnable project with core Minesweeper gameplay: ✅
2. Create a playable infinite Minesweeper game: ✅
3. Add simple roguelike elements to the game: ✅
4. Playtesting & bugtesting, finished & playable MVP created: ✅

## Features
*NOTE: This section may change as the project develops.*<br/>
*It does not cover standard Minesweeper mechanics, as those are implicitly included within MVP*

**Features to be implemented into the MVP (within deadline):**
- Progressively generated board: ✅
- Progressively incrementing mine chance: ✅
- Additional lives, with squares or other items that can give them: ✅
- Squares or other items that reduce incrementing mine chance in some way: ✅
- Different types of number squares, that reveal mine info in different ways than a standard number square: ✅
    - Cross square: ✅

            X...X
            .X.X.
            ..O..
            .X.X.
            X...X
    - Plus square: ✅

            ..X..
            ..X..
            XXOXX
            ..X..
            ..X..
    - Diamond square: ✅

            ..X..
            .X.X.
            X.O.X
            .X.X.
            ..X..
- Save & load functionality, persistent storage: ✅
- Upgrades ~~that persist between games~~: ✅
- Movable & zoomable camera: ✅
- Game can be restarted: ✅
- Basic functional UI: ✅
    - Main Menu / Title screen: ✅
    - Pause screen: ✅
    - SIdebar with... ✅
        - Game info: ✅
        - Upgrade display: ✅
- ~~Complete graphics, no placeholder textures: ⭕~~ *(Ended up just using standard minesweeper textures)*

**Extra features beyond MVP (after deadline):**
- User-friendly & polished UI: ❌
- Sounds & audio feedback: ❌
- Music: ❌
- Animations: ❌
- Configurable settings & settings menu: ❌
- In-game tutorial: ❌
- Chunk-based storage of the board, to actually support near-infinite board sizes: ❌
- More types of number squares: ❌
    - Knight square: ❌

            .X.X.
            X...X
            ..O..
            X...X
            .X.X.
    - Star square: ❌

            X...X
            ..X..
            .XOX.
            ..X..
            X...X
    - Distant square: ❌

            X.X.X
            .....
            X.O.X
            .....
            X.X.X
    - Arrow square: ❌

            ..X..  |  .....  |  ..X..  |  ..X..
            .XXX.  |  .....  |  .XX..  |  ..XX.
            XXOXX  |  XXOXX  |  XXO..  |  ..OXX
            .....  |  .XXX.  |  .XX..  |  ..XX.
            .....  |  ..X..  |  ..X..  |  ..X..
    - Line square: ❌

            (Just 8 squares in a straight line out from the square)
    - Dual line square: ❌

            (Just 4 squares in a straight line out from the square on opposite sides, for 8 squares total)
- Directional variants of some number squares, that offset or change direction of their influence: ❌
- Bomb square variants, with other negative effects: ❌
- Large squares, occupying more than one space: ❌

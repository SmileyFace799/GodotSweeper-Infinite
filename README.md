# GodotSweeper Infinite
<p>This is a re-make of my <a href="https://github.com/SmileyFace799/javasweeper_infinite">JavaSweeper Infinite</a> Java project, which again is a discontinued & unfinished re-make of my <code>Minesweeper Infinite</code> Python project (source code lost), which is quite simply Minesweeper in, but progressively generated & infinite.</p>

## Deadline
<p>The deadline for the MVP of the project is 6/1/2025. Anything outside the MVP will not be prioritized until after this deadline. Beyond the deadline, this will probably become more of a "passion project", where development will happen whenever I feel like it, without any time constraints.</p>

## Goals of this project
<p>The "end goal" of this project is simply infinite Minesweeper, but expanded with some unique features to adapt it into a roguelike. There's a few smaller goals along the way (listed below), which will be checked off as the project develops. More specific goals will be tracked using an issue board after MVP has been reached.</p>

<p><b>List of goals (within deadline):</b>
<ol>
    <li>Create a runnable project with core Minesweeper gameplay: ✅</li>
    <li>Create a playable infinite Minesweeper game: ✅</li>
    <li>Add simple roguelike elements to the game: ✅</li>
    <li>Playtesting & bugtesting, finished & playable MVP created: ✅</li>
</ol></p>

## Features
<p><i>NOTE: This section may change as the project develops.<br/>
It does not cover standard Minesweeper mechanics, as those are implicitly included within MVP</i></p>

<p><b>Features to be implemented into the MVP (within deadline):</b>
<ul>
    <li>Progressively generated board: ✅</li>
    <li>Progressively incrementing mine chance: ✅</li>
    <li>Additional lives, with squares or other items that can give them: ✅</li>
    <li>Squares or other items that reduce incrementing mine chance in some way: ✅</li>
    <li>Different types of number squares, that reveal mine info in different ways than a standard number square: ✅
    <ul>
<li>Cross square: ✅

        X...X
        .X.X.
        ..O..
        .X.X.
        X...X
</li><li>Plus square: ✅

        ..X..
        ..X..
        XXOXX
        ..X..
        ..X..
</li><li>Diamond square: ✅

        ..X..
        .X.X.
        X.O.X
        .X.X.
        ..X..
</li>
    </ul></li>
    <li>Save & load functionality, persistent storage: ✅</li>
    <li>Upgrades <del>that persist between games</del>: ✅</li>
    <li>Movable & zoomable camera: ✅</li>
    <li>Game can be restarted: ✅</li>
    <li>Basic functional UI: ✅<ul>
        <li>Main Menu / Title screen: ✅</li>
        <li>Pause screen: ✅</li>
        <li>SIdebar with... ✅<ul>
            <li>Game info: ✅</li>
            <li>Upgrade display: ✅</li>
        </ul></li>
    </ul></li>
    <li><del>Complete graphics, no placeholder textures: ⭕</del> <i>(Ended up just using standard minesweeper textures)</i></li>
</ul></p>

<p><b>Extra features beyond MVP (after deadline):</b>
<ul>
    <li>User-friendly & polished UI: ❌</li>
    <li>Sounds & audio feedback: ❌</li>
    <li>Music: ❌</li>
    <li>Animations: ❌</li>
    <li>Configurable settings & settings menu: ❌</li>
    <li>In-game tutorial: ❌</li>
    <li>Chunk-based storage of the board, to actually support near-infinite board sizes: ❌</li>
    <li>More types of number squares: ❌
    <ul>
<li>Knight square: ❌

        .X.X.
        X...X
        ..O..
        X...X
        .X.X.
</li><li>Star square: ❌

        X...X
        ..X..
        .XOX.
        ..X..
        X...X

</li><li>Distant square: ❌

        X.X.X
        .....
        X.O.X
        .....
        X.X.X
</li><li>Arrow square: ❌

        ..X..  |  .....  |  ..X..  |  ..X..
        .XXX.  |  .....  |  .XX..  |  ..XX.
        XXOXX  |  XXOXX  |  XXO..  |  ..OXX
        .....  |  .XXX.  |  .XX..  |  ..XX.
        .....  |  ..X..  |  ..X..  |  ..X..
</li><li>Line square: ❌

        (Just 8 squares in a straight line out from the square)
</li><li>Dual line square: ❌

        (Just 4 squares in a straight line out from the square on opposite sides, for 8 squares total)
</li>
    </ul></li>
    <li>Directional variants of some number squares, that offset or change direction of their influence: ❌</li>
    <li>Bomb square variants, with other negative effects: ❌</li>
    <li>Large squares, occupying more than one space: ❌</li>
</ul></p>

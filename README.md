# steiner-survivors-monogame

A top-down 2D survival game built in C# with the MonoGame framework. Survive waves of slimes and skulls, collect XP crystals, level up, and pick upgrades to last as long as you can.

Built as a three-person group project.

## Features

- **Main menu** with Play, Settings and Exit buttons and hover effects
- **Settings screen** to choose your movement controls: WASD or the arrow keys
- **Game states:** menu, settings, playing, paused, and game over, with restart or return to the menu
- **Pause** with the spacebar, plus a confirmation prompt before leaving to the menu
- **Soundtrack system:** menu music, a random track on demand, and music that pauses and resumes with the game
- **Combat:** auto-firing projectiles aimed at the mouse cursor, with two enemy types that chase the player
- **Progression:** enemies drop XP crystals, an XP bar, level-ups, and a choice of three upgrades
- **Camera** that follows the player across an animated grass map

## My Contributions

- Built the game state system (menu, settings, playing, paused and game over states)
- Designed and implemented the main menu and settings menu
- Added selectable movement controls (WASD or arrow keys)
- Integrated the in-game music system

## Controls

| Action | Input |
| --- | --- |
| Move | W A S D, or the arrow keys (choose in Settings) |
| Aim | Mouse (shooting is automatic) |
| Pause / resume | Space |
| Leave to menu while paused | Y (N stays paused) |
| Play a random song | Enter |
| Choose an upgrade after leveling up | 1, 2 or 3 |
| After game over | Y to restart, N to return to the menu |

## Built With

- C# and .NET 8
- MonoGame 3.8 (DesktopGL)

## Run Locally

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (version 8 or newer).
2. Clone the repo and open the folder:
   ```bash
   git clone https://github.com/mberry-dev/steiner-survivors-monogame.git
   cd steiner-survivors-monogame
   ```
3. Run the game:
   ```bash
   dotnet run
   ```

The first run takes a minute or two while MonoGame and its content tools download. In VS Code, the C# Dev Kit extension is recommended.

## Music

The original soundtrack used commercial songs, so the music files are **not included** in this repository. The game runs normally without them, just silently.

To add your own music:

1. Create a `Content/Audio` folder and put your `.mp3` files in it.
2. Add a `Content/Audio/Music.mgcb` file listing each track (see `Content/Content.mgcb` for the format, using `Mp3Importer` and `SongProcessor`).
3. Make sure the file names match the track names in `GameSoundtrack.cs` (for example `send_it.mp3` is the menu music).

Everything in `Content/Audio` is ignored by Git, so your music stays on your computer.

## Project Structure

```
steiner-survivors-monogame/
├── Game1.cs               # Game loop, game states, menus, input, drawing
├── GameSoundtrack.cs      # Music loading and playback
├── BackButton.cs          # Reusable menu button
├── ButtonPositioner.cs    # Centers the main menu buttons
├── Camera2D.cs            # Camera that follows the player
├── enemy.cs               # Enemy movement, health and XP drops
├── projectile.cs          # Player projectiles
├── xpitem.cs              # XP crystals
├── Content/               # Sprites, fonts and the MonoGame content file
└── SteinerSurvivors.csproj
```

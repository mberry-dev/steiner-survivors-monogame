# steiner-survivors-monogame

A top-down 2D survival game built in C# with the MonoGame framework. Survive waves of slimes and skulls, collect XP crystals, level up, and pick upgrades to last as long as you can.

Built as a three-person group project.

## Download and Play (Windows)

**[Download the latest version](https://github.com/mberry-dev/steiner-survivors-monogame/releases/latest)**: no install needed.

1. Download `SteinerSurvivors-win-x64.zip` from the release page.
2. Right-click the ZIP and choose **Extract All**.
3. Open the extracted folder and double-click **`SteinerSurvivors.exe`**.

If Windows shows "Windows protected your PC", click **More info**, then **Run anyway**. This appears for programs that aren't commercially code-signed. The music isn't included (see [Music](#music)), so the game is silent.

To run it from source instead, or on macOS or Linux, see [Run Locally](#run-locally).

## Features

- **Main menu** with Play, Settings and Exit buttons and hover effects
- **Settings screen** to choose your movement controls: WASD or the arrow keys
- **Game states:** menu, settings, playing, paused, and game over, with restart or return to the menu
- **Pause** with the spacebar, plus a confirmation prompt before leaving to the menu
- **Soundtrack system:** music when a game starts, a random track on demand, and music that pauses and resumes with the game
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

### What you need

- **Windows, macOS or Linux**
- **The .NET SDK, version 8 or newer.** Download it from https://dotnet.microsoft.com/download (choose the SDK, not just the runtime). To check that it's installed, open a terminal and run:
  ```bash
  dotnet --version
  ```
  It should print a version number starting with 8 or higher.
- **Git** (optional, for cloning). Download it from https://git-scm.com. You can also download the code as a ZIP instead.
- **VS Code** (optional) with the **C# Dev Kit** extension, if you want to edit the code.

### Step 1: Get the code

**With Git:**
```bash
git clone https://github.com/mberry-dev/steiner-survivors-monogame.git
cd steiner-survivors-monogame
```

**Without Git:** on the repo page, click **Code → Download ZIP**, then extract the ZIP. On Windows, right-click the ZIP and choose **Extract All**. Open a terminal inside the extracted folder (the one that contains `SteinerSurvivors.csproj`).

### Step 2: Run the game

```bash
dotnet run
```

The first run takes a few minutes. .NET downloads MonoGame and its content tools, then builds the game's images and fonts. Later runs start in seconds. The game opens in its own window.

### Using VS Code instead

1. Open VS Code and choose **File → Open Folder**, then select the project folder.
2. Install the **C# Dev Kit** extension if VS Code suggests it.
3. Open the terminal with `` Ctrl + ` `` and run `dotnet run`.

### Troubleshooting

- **"dotnet: command not found":** the .NET SDK isn't installed, or the terminal was opened before installing it. Install the SDK, then close and reopen the terminal.
- **"came from another computer and might be blocked" (Windows, ZIP download):** Windows blocks files downloaded from the internet. In the project folder, run this in PowerShell, then try again:
  ```powershell
  Get-ChildItem -Recurse | Unblock-File
  ```
- **"The command dotnet tool restore exited with code 1":** run `dotnet tool restore` on its own to see the real error. It's usually the blocked-files issue above, or no internet connection on the first run.
- **The game is silent:** that's expected. The music isn't included in this repository (see below).

## Music

The original soundtrack used commercial songs, so the music files are **not included** in this repository. The game runs normally without them, just silently.

To add your own music:

1. Create a `Content/Audio` folder and put your `.mp3` files in it.
2. Add a `Content/Audio/Music.mgcb` file listing each track (see `Content/Content.mgcb` for the format, using `Mp3Importer` and `SongProcessor`).
3. Make sure the file names match the track names in `GameSoundtrack.cs` (for example `send_it.mp3` plays when a game starts).

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

# URBAN ESCAPE: RAILWAY ESCAPE 🚂💨

**Capstone Project — School of Computer Science & Artificial Intelligence**  
**Student:** V. Anushka (2303A51176)  
**Guide:** Dr. Vijaya Chandra Jadala, Assistant Professor, School of CS&AI  
**University:** SR University, Warangal, Telangana  
**Theme:** Railway Track (Endless Runner)

---

## 🎮 Game Overview

**Railway Escape** is an endless runner 3D game built in Unity (Unity 6 / Universal Render Pipeline). The player auto-runs along a 3-lane railway track system at dusk, dodging oncoming trains, railway crossing barriers, and trackside signals while collecting gold coins and ruby red gems to maximize their distance and score.

---

## 🕹️ Controls

| Action | Key / Control | Description |
| :--- | :--- | :--- |
| **Move Left** | `A` or `Left Arrow` | Moves player left across the 3 railway track lanes (bounded within `[-5.5, +5.5]`) |
| **Move Right** | `D` or `Right Arrow` | Moves player right across the 3 railway track lanes |
| **Jump** | `Spacebar` | Leaps over railway barriers and hazards |
| **Pause** | `||` Button (Top Right) | Opens Pause Menu with Resume, Restart, and Main Menu |

---

## 🏗️ Project Architecture & Scenes

The game implements the complete 4-scene flow specified in the project report:

1. **`MainMenu.unity`** (`Assets/Scenes/MainMenu.unity`)
   - Shows top-left HUD displaying all-time saved stats (`GEMSAVE`, `COINSAVE`, `DISTANCESAVE`).
   - Themed 3D preview of railway tracks, moonlit sky, and train.
   - "Start Game" button leading to Stage Selection.
2. **`StageSelect.unity`** (`Assets/Scenes/StageSelect.unity`)
   - Displays a rotating 3D diorama pedestal of the Railway Escape theme.
   - "PLAY" button to proceed to instructions and gameplay.
3. **`Information.unity`** (`Assets/Scenes/Information.unity`)
   - Visual control guide showing `A / D`, `Spacebar`, and objectives.
   - Automatically loads `GamePlay` via `LoadToStage.cs` with smooth fade-out.
4. **`GamePlay.unity`** (`Assets/Scenes/GamePlay.unity`)
   - 3-lane endless railway track procedural generation (`SegmentGenerator.cs` & `RailwayThemeBuilder.cs`).
   - Third-person dynamic camera follow (`CameraFollow.cs`).
   - Dynamic hazards: 3D Trains, striped railway hurdles, and traffic signal posts.
   - Spinning gold coins and red gems with shrink-and-rise pickup effects.
   - Live HUD with counters for Gems, Coins, and Distance (`MasterLevelInfo.cs`).
   - Pause menu and game-over collision restart loop (`CollisionDetect.cs`).

---

## 📜 Complete Scripts (11 PPT Scripts + Helpers)

All scripts are located in `Assets/Scripts/`:

| # | Script Name | Purpose |
| :-: | :--- | :--- |
| 1 | **`MainMenuControl.cs`** | Start game button handler; loads and displays saved coins, gems, and distance. |
| 2 | **`PlayerMovement.cs`** | Auto-forward run along Z-axis, left/right steering clamped to `[-5.5, +5.5]`, jumping, and distance counter. |
| 3 | **`CollectCoin.cs`** | Coin trigger; adds +1 coin, plays sound, executes shrink-and-rise animation, and destroys item. |
| 4 | **`GemCollect.cs`** | Gem trigger; adds +1 gem, plays chime, executes shrink-and-rise animation, and destroys item. |
| 5 | **`CollectableRotate.cs`** | Continuous rotation (spin) for coins and gems so players notice them. |
| 6 | **`SaveLoad.cs`** | Persistent storage using `PlayerPrefs` (`COINSAVE`, `GEMSAVE`, `DISTANCESAVE`). |
| 7 | **`StageControls.cs`** | Stage navigation between Main Menu, Stage Select, and Information. |
| 8 | **`MasterLevelInfo.cs`** | Runtime tracker for current coins, gems, and distance; updates HUD in real-time. |
| 9 | **`CollisionDetect.cs`** | Obstacle collision detector; triggers camera shake, stops runner, saves progress, fades out, and restarts scene. |
| 10 | **`SegmentGenerator.cs`** | Spawns endless railway segments ahead every 3 seconds and recycles past segments. |
| 11 | **`LoadToStage.cs`** | Information screen countdown and fade transition to gameplay. |
| * | **`RailwayThemeBuilder.cs`** | Generates 3D railway track meshes, ballast, rails, ties, trains, barriers, and signals. |
| * | **`CameraFollow.cs`** | Smooth third-person chase camera. |
| * | **`PauseMenu.cs`** | In-game pause, resume, restart, and menu return. |

---

## 📦 Prefabs (`Assets/Prefabs/`)

- `Player.prefab`
- `Segment.prefab`
- `Coin.prefab`
- `Gem.prefab`
- `Obstacle.prefab`
- `TrainObstacle.prefab`
- `BarrierObstacle.prefab`
- `SignalObstacle.prefab`

---

## 🚀 How to Run & Play

### Option 1: Direct Play via Standalone Build (No Unity required)
Run:
```
D:\RailwayTrack\Build\RailwayEscape.exe
```

### Option 2: Open in Unity Editor
1. Open **Unity Hub**.
2. Click **Add** -> **Add project from disk**.
3. Select `D:\RailwayTrack`.
4. Open the project with **Unity 6 (6000.3.23f1)**.
5. In the Project window, open `Assets/Scenes/MainMenu.unity` and click **Play** (▶️).


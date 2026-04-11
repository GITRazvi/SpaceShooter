# SpaceShooter

A Unity-based space shooter game featuring player controls, enemies, power-ups, scoring, and basic UI feedback.

> Note: The Unity editor version used for this project is recorded in ProjectSettings/ProjectVersion.txt. Check that file if you need an exact editor version.

## Implemented Systems (by script)

- Player.cs
  - Movement: omnidirectional movement using Horizontal/Vertical axes with movement clamped to X in [-15, 15] and Y in [-9, 6]. Horizontal wrapping was removed; the player stays within bounds.
  - Speed: base speed is serialized (default 4.0f) with a speed multiplier (default 2.0x) applied while the Speed Boost power-up is active. The player's base speed is increased by +0.5 every 30 seconds via InvokeRepeating (starts after 5s).
  - Shooting: fires a laser prefab with a configurable fire rate (default 0.5s). Supports a triple-shot power-up which instantiates a triple-shot prefab at the player position and lasts approximately 5 seconds.
  - Damage & Lives: a lives counter (default 3). Damage has a cooldown (default 0.5s) to prevent rapid consecutive hits. When lives < 1 the SpawnManager is notified and the player GameObject is destroyed. Damage triggers a brief visual flash using a coroutine that temporarily changes sprite alpha.
  - Power-ups: methods to activate Triple Shot, Speed Boost, and Shield. Triple Shot and Speed Boost are temporary (~5s). Shield spawns a shield prefab as a child of the player and absorbs the next hit; additional shield activations are ignored while a shield exists.

- Enemy.cs
  - Movement: enemies move downward at a configurable speed (serialized; default 2.0f) and when they pass below y = -8 they are repositioned to the top at y = 7 with a random X between -8 and 8.
  - HP & scaling: serialized enemy HP (default 1.0f). Every 30 seconds (starting after 5s) IncreaseStats is called to add +1.0 HP and +0.5 speed for basic difficulty scaling.
  - Collisions: handles collisions with Player (calls Player.Damage and destroys an enemy GameObject) and Laser (reduces enemy HP by laser._laserDamage and destroys enemy when HP <= 0). When destroyed by a laser, Score.Instance.AddScore() is called.
  - Kill counter behavior: the enemy tracks a local count of kills; when count reaches 10 it calls Laser.LaserDamage() and resets the counter (current implementation calls LaserDamage on the last-colliding Laser instance).
  - Note: the enemy currently destroys an enemy GameObject by tag (Destroy(GameObject.FindWithTag("Enemy"))), which removes the first GameObject found with that tag; this is how the current code removes enemies on collisions.

- Laser.cs
  - Movement: lasers move upward at a configurable speed (default 8.0f) and are intended to destroy themselves when leaving the top of the screen.
  - Damage: exposes a public _laserDamage field (default 1.0f) and a LaserDamage() method (used by the Enemy logic to increase laser damage once a kill threshold is reached).
  - Parenting: triple-shot lasers are spawned from a parent triple-shot prefab — parented or grouped lasers will manage their own destruction so that the whole shot is cleaned up when off-screen.

- Powerup.cs
  - Movement: power-ups move downward and are destroyed if they pass below the bottom of the screen.
  - Types: a powerupID determines the effect (e.g., Triple Shot, Speed Boost, Shield). On collision with the Player, the corresponding Player method is called and the power-up is destroyed.

- Score.cs
  - Singleton: Score implements a simple singleton (Score.Instance) for global access.
  - Points: configurable points per enemy (default 10). AddScore increments the total score and updates an assigned UI display.
  - Utility: methods to get and reset the current score.

- ShowHp.cs
  - UI: updates an Image or SpriteRenderer to show current player lives using serialized sprites for 3, 2, 1, and 0 lives.
  - Player reference: requires a Player reference to read the current lives. If not assigned, the component disables itself with a warning.
  - Efficient updates: only updates visuals when the player's life count changes.

- SpawnManager.cs
  - Enemy spawning: repeatedly instantiates enemy prefabs at the top of the screen at random X positions with a spawn interval (default 5s). Enemies spawn relative to the player's position: Y = player.Y + 8, X = player.X + random offset in [-7, 7].
  - Power-up spawning: spawns random power-ups at random intervals (commonly between 3 and 8 seconds) using the same relative positioning logic as enemies.
  - Containers: newly spawned enemies/powerups are parented under configured container GameObjects to keep the hierarchy organized.
  - Stop spawning: when the player dies the SpawnManager stops spawning further enemies and power-ups.

## Gameplay Balance (defaults seen in scripts)
- Player base speed: 4.0f (serialized)
- Player speed multiplier (speed boost): 2.0x
- Player fire rate: 0.5 seconds
- Laser speed: 8.0f
- Laser damage: 1.0f (increases via Laser.LaserDamage())
- Enemy base speed: 2.0f (serialized)
- Enemy HP scaling: +1 HP every 30 seconds (starts after 5s)
- Enemy speed scaling: +0.5 speed every 30 seconds (starts after 5s)
- Power-up durations: Triple Shot and Speed Boost last ~5 seconds
- Damage cooldown: 0.5 seconds between player hits
- Enemy spawn interval: ~5 seconds (see SpawnManager)
- Power-up spawn interval: random between 3 and 8 seconds (see SpawnManager)
- Points per enemy: 10 (default in Score)

## Notes & Where to look in the project
- Player behavior and power-up handling: Assets/Scripts/Player.cs
- Enemy behavior and scaling: Assets/Scripts/Enemy.cs
- Laser behavior and damage: Assets/Scripts/Laser.cs
- Spawning & game flow: Assets/Scripts/SpawnManager.cs
- Scoring & UI hooks: Assets/Scripts/Score.cs
- Project Unity version: ProjectSettings/ProjectVersion.txt


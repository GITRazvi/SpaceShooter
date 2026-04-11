# SpaceShooter

A Unity-based space shooter game featuring player controls, enemies, power-ups, scoring, and basic UI feedback.The used Unity version is 6000.0.32f1

## Implemented Systems (by script)

- Player.cs
  - Movement: omnidirectional movement using Horizontal/Vertical axes with screen wrapping on X and clamped Y between -3.8 and 0.
  - Speed: base speed (serialized) with a multiplier for speed power-up. Speed increases by 0.5 every 30 seconds (starts after 5s).
  - Shooting: fires a laser prefab with a configurable fire rate (default 0.5s). Supports a triple-shot power-up which lasts 5 seconds.
  - Damage & Lives: a lives counter (default 3). Damage has a cooldown (default 0.5s) to prevent rapid consecutive hits. When lives < 1 the SpawnManager is notified and the player GameObject is destroyed.
  - Power-ups: methods to activate Triple Shot, Speed Boost, and Shield. Each temporary effect uses a coroutine to time out (5s for triple shot and speed boost). Shield spawns a shield prefab as a child and absorbs one hit.

- Enemy.cs
  - Movement: enemies move downward at a configurable speed and respawn at the top with a random X when passing below -8 on Y.
  - HP: serialized enemy HP that increases by 1.0 every 30 seconds (starts after 5s) for difficulty scaling.
  - Collisions: handles collisions with Player (damages player and destroys an enemy) and Laser (reduces enemy HP by laser damage and destroys enemy when HP <= 0).
  - Score: when destroyed by a laser, the Score singleton is used to add points.

- Laser.cs
  - Movement: lasers move upward at a configurable speed and destroy themselves when leaving the top of the screen (y > 8).
  - Parented lasers (e.g., from triple-shot) will destroy their parent when exiting the screen.
  - Damage scaling: Laser exposes a method to increase its damage by 1.0f.

- Powerup.cs
  - Movement: power-ups move downward and are destroyed if they pass below the bottom of the screen.
  - Types: a powerupID determines the effect (0 = Triple Shot, 1 = Speed Boost, 2 = Shield). On collision with the Player, the corresponding Player method is called and the power-up is destroyed.

- Score.cs
  - Singleton: Score implements a simple singleton (Score.Instance) for global access.
  - Points: configurable points per enemy (default 10). AddScore increments the total score and updates an assigned TextMeshProUGUI display.
  - Utility: methods to get and reset the current score.

- ShowHp.cs
  - UI: updates an Image or SpriteRenderer to show current player lives using serialized sprites for 3, 2, 1, and 0 lives.
  - Player reference: requires a Player reference to read the current lives. If not assigned, the component disables itself with a warning.
  - Efficient updates: only updates visuals when the player's life count changes.

- SpawnManager.cs
  - Enemy spawning: repeatedly instantiates enemy prefabs at the top of the screen at random X positions with a spawn interval (default 5s).
  - Power-up spawning: spawns random power-ups at random intervals between 3 and 8 seconds.
  - Containers: newly spawned enemies/powerups are parented under configured container GameObjects to keep the hierarchy organized.
  - Stop spawning: when the player dies the SpawnManager stops spawning further enemies and power-ups.

## Gameplay Balance (defaults seen in scripts)
- Player base speed: serialized (default 4.0f in code)
- Player fire rate: 0.5 seconds
- Laser speed: serialized (default 8.0f)
- Enemy speed: serialized (default 4.0f)
- Enemy HP scaling: +1 HP every 30 seconds (starts after 5s)
- Power-up durations: Triple Shot and Speed Boost last ~5 seconds
- Damage cooldown: 0.5 seconds between player hits
- Enemy spawn interval: ~5 seconds
- Power-up spawn interval: random between 3–8 seconds
- Points per enemy: 10 (default)

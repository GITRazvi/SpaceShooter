using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _PowerupContainer;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private GameObject _bossContainer; // Parent container for spawned bosses
    [SerializeField]
    private float _bossSpawnTimer = 30f;
    [SerializeField]
    private GameObject[] powerups;

    // New: boss HP configuration and UI prefab/canvas
    [SerializeField]
    private float _initialBossHP = 30f;
    [SerializeField]
    private float _bossHPIncrement = 30f;
    [SerializeField]
    private GameObject _hpSliderPrefab;
    [SerializeField]
    private Transform _uiCanvas;

    private bool _stopSpawning = false;
    private Player _player;
    private bool _isBossFight = false;
    private GameObject _currentBoss = null;
    private float _bossSpawnCount = 0f;
    
    // Player movement bounds: x in [-24, 24], y in [-19, 26]
    private const float SPAWN_MIN_X = -23f;
    private const float SPAWN_MAX_X = 23f;
    private const float SPAWN_MIN_Y = -18f;
    private const float SPAWN_MAX_Y = 25f;
    
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_bossPrefab == null)
        {
            Debug.LogError("Boss prefab is not assigned in SpawnManager!");
        }
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnBossRoutine());
    }

    private void Update()
    {
    }


    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            // Skip spawning if boss fight is active
            if (_isBossFight == false)
            {
                // Spawn enemy relative to player position
                // Y position: player's Y + 8
                // X position: player's X + random value between -7 and +7
                Vector3 playerPos = _player.transform.position;
                float spawnX = playerPos.x + Random.Range(-7f, 7f);
                float spawnY = playerPos.y + 8f;

                // Clamp spawn position to player movement bounds
                spawnX = Mathf.Clamp(spawnX, SPAWN_MIN_X, SPAWN_MAX_X);
                spawnY = Mathf.Clamp(spawnY, SPAWN_MIN_Y, SPAWN_MAX_Y);

                Vector3 posToSpawn = new Vector3(spawnX, spawnY, 0);

                GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 8.0f));

            // Skip spawning if boss fight is active
            if (_isBossFight == false)
            {
                // Spawn powerup relative to player position (same logic as enemies)
                Vector3 playerPos = _player.transform.position;
                float spawnX = playerPos.x + Random.Range(-7f, 7f);
                float spawnY = playerPos.y + 8f;

                // Clamp spawn position to player movement bounds
                spawnX = Mathf.Clamp(spawnX, SPAWN_MIN_X, SPAWN_MAX_X);
                spawnY = Mathf.Clamp(spawnY, SPAWN_MIN_Y, SPAWN_MAX_Y);

                Vector3 posToSpawn = new Vector3(spawnX, spawnY, 0);

                int randomIndex = Random.Range(0, powerups.Length);
                GameObject newPowerup = Instantiate(powerups[randomIndex], posToSpawn, Quaternion.identity);
                newPowerup.transform.parent = _PowerupContainer.transform;
            }

            yield return null;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    IEnumerator SpawnBossRoutine()
    {
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(_bossSpawnTimer);

            // Only spawn a new boss if no boss currently exists
            if (_currentBoss == null && _bossPrefab != null && _player != null)
            {
                // Spawn boss above player, similar to how enemies spawn
                Vector3 spawnPosition = _player.transform.position;
                spawnPosition.y = _player.transform.position.y + 20f;

                // Use the prefab's rotation so any orientation set on the prefab (e.g. upside-down) is preserved
                GameObject newBoss = Instantiate(_bossPrefab, spawnPosition, _bossPrefab.transform.rotation);

                // Parent boss to designated boss container if assigned
                if (_bossContainer != null)
                {
                    newBoss.transform.parent = _bossContainer.transform;
                }

                _currentBoss = newBoss;
                _isBossFight = true;

                // Calculate boss HP based on spawn count
                _bossSpawnCount++;
                float maxHP = _initialBossHP + (_bossSpawnCount - 1) * _bossHPIncrement;
                float bossHP = maxHP;

                // Set up HP bar for the boss (instantiate under UI Canvas if available)
                GameObject hpBarInstance = null;
                BossHPSlider hpSliderComponent = null;

                if (_hpSliderPrefab != null)
                {
                    hpBarInstance = Instantiate(_hpSliderPrefab);

                    if (_uiCanvas != null)
                    {
                        hpBarInstance.transform.SetParent(_uiCanvas, false);
                    }
                    else
                    {
                        Canvas foundCanvas = Object.FindAnyObjectByType<Canvas>();
                        if (foundCanvas != null)
                        {
                            hpBarInstance.transform.SetParent(foundCanvas.transform, false);
                        }
                        else
                        {
                            Debug.LogWarning("SpawnManager: No UI Canvas assigned and none found in scene. HP bar will be in root of scene hierarchy.");
                        }
                    }

                    hpSliderComponent = hpBarInstance.GetComponent<BossHPSlider>();
                    if (hpSliderComponent == null)
                    {
                        Debug.LogError("SpawnManager: HP slider prefab does not have BossHPSlider component!");
                    }
                }
                else
                {
                    Debug.LogWarning("SpawnManager: _hpSliderPrefab is not assigned. Boss will have no HP bar.");
                }

                // Notify the boss behaviour that it has been spawned
                BossBehaviour bossBehaviour = newBoss.GetComponent<BossBehaviour>();
                if (bossBehaviour != null)
                {
                    bossBehaviour.SetPlayerReference(_player);
                    bossBehaviour.SetSpawnManager(this);
                    bossBehaviour.OnSpawned(bossHP, maxHP, hpBarInstance, hpSliderComponent, _bossSpawnCount);
                }

                // Notify camera to zoom out for boss fight
                GameObject cameraObj = GameObject.Find("Main Camera");
                if (cameraObj != null)
                {
                    var cameraScript = cameraObj.GetComponent<Camera>();
                    if (cameraScript != null)
                    {
                        cameraScript.ZoomOutForBoss();
                    }
                    else
                    {
                        Debug.LogError("SpawnManager: Camera component not found on Main Camera!");
                    }
                }
                else
                {
                    Debug.LogError("SpawnManager: Main Camera not found!");
                }

            }
            else if (_currentBoss == null)
            {
                Debug.LogError($"Cannot spawn boss: _bossPrefab={_bossPrefab}, _player={_player}");
            }
        }
    }

    public void OnBossDefeated()
    {
        _currentBoss = null;
        _isBossFight = false;
        Debug.Log("Boss defeated! Normal spawning resumed.");
    }

    public bool IsBossFight()
    {
        return _isBossFight;
    }

}

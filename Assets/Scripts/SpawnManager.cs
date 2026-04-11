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
    private bool _stopSpawning = false;
    [SerializeField]
    private GameObject[] powerups;
    private Player _player;
    
    // Player movement bounds: x in [-15, 15], y in [-9, 6]
    private const float SPAWN_MIN_X = -14f;
    private const float SPAWN_MAX_X = 14f;
    private const float SPAWN_MIN_Y = -8f;
    private const float SPAWN_MAX_Y = 5f;
    
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    private void Update()
    {
    }


    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
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

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 8.0f));
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
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}

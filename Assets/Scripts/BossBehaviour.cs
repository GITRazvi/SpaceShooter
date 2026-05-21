using UnityEngine;
using System.Collections;

public class BossBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _initialBossHP = 30f;
    [SerializeField]
    private GameObject _bossProjectilePrefab;
    [SerializeField]
    private float _fireRate = 1.5f;
    [SerializeField]
    private GameObject _hpSliderPrefab;

    private float _currentTime = 0f;
    private float _bossHP;
    private float _spawnCount = 0f;
    private float _nextFire = 0.0f;
    private Player _player;
    private bool _hasSpawned = false;
    private SpawnManager _spawnManager;
    private BossHPSlider _hpSlider;
    private GameObject _hpSliderInstance;

    public void SetPlayerReference(Player player)
    {
        _player = player;
    }

    public void SetSpawnManager(SpawnManager spawnManager)
    {
        _spawnManager = spawnManager;
    }

    void Update()
    {
        if (_hasSpawned)
        {
            // Keep boss Y position at player's Y + 20
            if (_player != null)
            {
                Vector3 currentPosition = transform.position;
                currentPosition.y = _player.transform.position.y + 20f;
                transform.position = currentPosition;
            }

            // Fire projectiles at player
            if (Time.time > _nextFire && _player != null && _bossProjectilePrefab != null)
            {
                FireProjectile();
                _nextFire = Time.time + _fireRate;
            }
        }
    }

    private void FireProjectile()
    {
        // Fire multiple projectiles in a spread pattern
        for (int i = -1; i <= 1; i++)
        {
            Vector3 firePosition = transform.position + new Vector3(i * 1.5f, 0, 0);
            Instantiate(_bossProjectilePrefab, firePosition, Quaternion.identity);
        }
    }

    private void SpawnBoss()
    {
        _spawnCount++;
        _bossHP = _initialBossHP + (_spawnCount - 1) * 30f;
        _hasSpawned = true;

        // Position boss above player
        if (_player != null)
        {
            Vector3 spawnPosition = _player.transform.position;
            spawnPosition.y = _player.transform.position.y + 20f;
            transform.position = spawnPosition;
        }

        // Rotate boss 180 degrees to be upside down
        transform.rotation = Quaternion.Euler(0, 0, 180f);

        // Spawn and show boss HP bar
        if (_hpSliderPrefab != null)
        {
            _hpSliderInstance = Instantiate(_hpSliderPrefab);
            _hpSlider = _hpSliderInstance.GetComponent<BossHPSlider>();
            if (_hpSlider != null)
            {
                Debug.Log("BossBehaviour: Activating HP bar");
                _hpSlider.ActivateHPBar(_bossHP, _initialBossHP + (_spawnCount - 1) * 30f);
            }
            else
            {
                Debug.LogError("BossBehaviour: HP slider prefab does not have BossHPSlider component!");
            }
        }
        else
        {
            Debug.LogError("BossBehaviour: _hpSliderPrefab is not assigned in the Inspector!");
        }

        // Notify camera to zoom out
        Camera cameraScript = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (cameraScript != null)
        {
            cameraScript.ZoomOutForBoss();
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    public void InitializeBoss()
    {
        SpawnBoss();
    }

    public float GetHP()
    {
        return _bossHP;
    }

    public void TakeDamage(float damage)
    {
        _bossHP -= damage;

        // Update HP bar
        if (_hpSlider != null)
        {
            _hpSlider.UpdateHPBar(_bossHP, _initialBossHP + (_spawnCount - 1) * 30f);
        }

        if (_bossHP <= 0)
        {
            DefeatedBoss();
        }
    }

    private void DefeatedBoss()
    {
        gameObject.SetActive(false);
        _hasSpawned = false;
        _currentTime = 0f;

        // Destroy HP bar
        if (_hpSliderInstance != null)
        {
            Debug.Log("BossBehaviour: Deactivating HP bar");
            Destroy(_hpSliderInstance);
            _hpSliderInstance = null;
            _hpSlider = null;
        }

        if (_spawnManager != null)
        {
            _spawnManager.OnBossDefeated();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision with player laser
        if (collision.CompareTag("Laser"))
        {
            Laser laser = collision.GetComponent<Laser>();
            if (laser != null)
            {
                TakeDamage(laser._laserDamage);
                Destroy(collision.gameObject);
            }
        }
    }
}

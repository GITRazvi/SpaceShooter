using UnityEngine;
using System.Collections;

public class BossBehaviour : MonoBehaviour
{
    //[SerializeField]
    //private float _initialBossHP = 30f; // kept for inspector reference but SpawnManager will provide actual values
    [SerializeField]
    private GameObject _bossProjectilePrefab;
    [SerializeField]
    private float _fireRate = 1.5f;
    [SerializeField]
    private GameObject _hpSliderPrefab;
    [SerializeField]
    private Transform _uiCanvas; 

    private float _bossHP;
    private float _maxHP;
    private float _spawnCount = 0f;
    private float _nextFire = 0.0f;
    private Player _player;
    private bool _hasSpawned = false;
    private SpawnManager _spawn_manager;
    private BossHPSlider _hpSlider;
    private GameObject _hpSliderInstance;

    public void SetPlayerReference(Player player)
    {
        _player = player;
    }

    public void SetSpawnManager(SpawnManager spawnManager)
    {
        _spawn_manager = spawnManager;
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

    // Called by SpawnManager after it instantiates and parents the boss and HP bar
    public void OnSpawned(float bossHP, float maxHP, GameObject hpBarInstance, BossHPSlider hpSliderComponent, float spawnCount)
    {
        _spawnCount = spawnCount;
        _bossHP = bossHP;
        _maxHP = maxHP;
        _hpSliderInstance = hpBarInstance;
        _hpSlider = hpSliderComponent;
        _hasSpawned = true;

        // Ensure HP bar shows correct values
        if (_hpSlider != null)
        {
            _hpSlider.ActivateHPBar(_bossHP, _maxHP);
        }
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
            _hpSlider.UpdateHPBar(_bossHP, _maxHP);
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

        // Destroy HP bar
        if (_hpSliderInstance != null)
        {
            
            Destroy(_hpSliderInstance);
            _hpSliderInstance = null;
            _hpSlider = null;
        }

        // Award points for defeating the boss
        if (Score.Instance != null)
        {
            Score.Instance.AddScore(100);
        }

        if (_spawn_manager != null)
        {
            _spawn_manager.OnBossDefeated();
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

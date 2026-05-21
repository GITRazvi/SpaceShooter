using UnityEngine;

public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 0.8f;
    [SerializeField]
    private float _projectileDamage = 1.0f;
    [SerializeField]
    private float _maxDistance = 100f;

    private Transform _playerTransform;
    private Vector3 _startPosition;

    void Start()
    {
        // Find and cache the player transform
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyProjectile: Player not found!");
            Destroy(gameObject);
        }

        _startPosition = transform.position;
    }

    void Update()
    {
        if (_playerTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate direction to player
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;

        // Move towards player
        transform.Translate(directionToPlayer * _speed * Time.deltaTime);

        // Check if projectile has traveled too far
        if (Vector3.Distance(transform.position, _startPosition) > _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public float GetDamage()
    {
        return _projectileDamage;
    }

    public void IncreaseDamage(float amount)
    {
        _projectileDamage += amount;
    }
}

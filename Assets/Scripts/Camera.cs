using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform;
    [SerializeField]
    private float _followDistance = 5f;
    [SerializeField]
    private float _smoothSpeed = 5f;
    
    private Vector3 _offset;

    void Start()
    {
        if (_playerTransform == null)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Camera: Player GameObject not found!");
                enabled = false;
                return;
            }
        }
        
        // Set initial offset based on follow distance
        _offset = new Vector3(0, 0, -_followDistance);
    }

    void Update()
    {
        if (_playerTransform == null)
            return;

        // Calculate target position
        Vector3 targetPosition = _playerTransform.position + _offset;
        
        // Smoothly move camera towards target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
    }
}

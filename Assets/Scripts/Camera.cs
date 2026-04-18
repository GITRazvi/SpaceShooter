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
        
        _offset = new Vector3(0, 0, -_followDistance);
    }

    void Update()
    {
        if (_playerTransform == null)
            return;

        Vector3 targetPosition = _playerTransform.position + _offset;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
    }
}

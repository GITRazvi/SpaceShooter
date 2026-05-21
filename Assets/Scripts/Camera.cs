using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform;
    [SerializeField]
    private float _followDistance = 5f;
    [SerializeField]
    private float _smoothSpeed = 5f;
    [SerializeField]
    private float _bossZoomDistance = 15f;
    [SerializeField]
    private float _zoomTransitionDuration = 1f;

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

    public void ZoomOutForBoss()
    {
        Debug.Log("ZoomOutForBoss() called!");
        StopCoroutine(nameof(SmoothZoomTransition));
        StartCoroutine(SmoothZoomTransition(_bossZoomDistance));
    }

    private IEnumerator SmoothZoomTransition(float targetDistance)
    {
        float startDistance = Mathf.Abs(_offset.z);
        float elapsedTime = 0f;

        Debug.Log($"Starting zoom transition: startDistance={startDistance}, targetDistance={targetDistance}");

        while (elapsedTime < _zoomTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float newDistance = Mathf.Lerp(startDistance, targetDistance, elapsedTime / _zoomTransitionDuration);
            _offset = new Vector3(0, 0, -newDistance);
            Debug.Log($"Zoom progress: {elapsedTime / _zoomTransitionDuration:P0}, offset.z={_offset.z}");
            yield return null;
        }

        // Ensure final position is exact
        _offset = new Vector3(0, 0, -targetDistance);
        Debug.Log($"Zoom complete! Final offset.z={_offset.z}");
    }
}

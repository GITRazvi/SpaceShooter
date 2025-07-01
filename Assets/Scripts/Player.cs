using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f; // f comes from float 
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _lives=3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private GameObject _speedup;
    [SerializeField]
    private bool _isSpeedupActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldPrefab;

    void Start()
    {
        transform.position = new Vector3(0,0,0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
            
        if (_spawnManager != null)
        {
            Debug.Log("The spawn manager is null");
        }
    }
    void Update()
    {
        calculateMovement();


        if (Input.GetKeyDown(KeyCode.Space)&&Time.time > _nextFire) {
            fireLaser();
        }
       
    }

    void calculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if(_isSpeedupActive == false)
            transform.Translate(direction * _speed * Time.deltaTime);
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime * _speedMultiplier);
        }
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11)
        {
            transform.position = new Vector3(-11, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -11)
        {
            transform.position = new Vector3(11, transform.position.y, transform.position.z);
        }

    }
    void fireLaser()
    {
            _nextFire = Time.time + _fireRate;
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShot, transform.position, quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), quaternion.identity);
        }

    }

    public void Damage()
    {
        if (_isShieldActive == false)
            _lives--;
        else 
        {
            _isShieldActive = false;
            Transform shield = transform.Find(_shieldPrefab.name + "(Clone)");
            if (shield != null)
            {
                Destroy(shield.gameObject);
            }
            return; 
        }
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());

    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        while (_isTripleShotActive)
        {
            yield return new WaitForSeconds(5.0f);
            _isTripleShotActive=false;
        }
    }
    public void SpeedupActive()
    {
        _isSpeedupActive = true;
        StartCoroutine(SpeedupPowerDownRoutine());

    }
    IEnumerator SpeedupPowerDownRoutine()
    {
        while (_isSpeedupActive)
        {   
            yield return new WaitForSeconds(5.0f);
            _isSpeedupActive = false;
        }
    }
    public void ShieldActive()
    {
        _isShieldActive = true;
        GameObject shieldInstance = Instantiate(_shieldPrefab, transform.position, quaternion.identity);
        shieldInstance.transform.parent = this.transform;
    }
}
 
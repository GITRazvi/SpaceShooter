using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;  
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private float _damageCooldown = 0.5f;
    private float _nextDamageTime = 0.0f;
    [SerializeField]
    private int _lives=3;
    public int Lives => _lives; // public read-only property for current lives
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
        //Increase the speed of the player every 30 seconds, starting from 5 seconds into the game, this can be changed and is not final just for balancing purposes
        InvokeRepeating(nameof(IncreaseSpeed),5f,30f);
        transform.position = new Vector3(0,0,0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    void IncreaseSpeed()
        {
            _speed += 0.5f;
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

        // Wrap horizontally at x = -14 and x = 14, clamp vertical movement to y in [-9,6]
        float posX = transform.position.x;
        float posY = Mathf.Clamp(transform.position.y, -9f, 6f);

        if (posX <= -14f)
        {
            posX = 14f;
        }
        else if (posX >= 14f)
        {
            posX = -14f;
        }

        transform.position = new Vector3(posX, posY, 0);

        // Removed horizontal clamping so player wraps around world edges
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
        if (Time.time < _nextDamageTime)
        {
            return; // Cooldown is still active, ignore damage
        }
        
        _nextDamageTime = Time.time + _damageCooldown;
        
        if (_isShieldActive == false)
        {
            _lives--;
            StartCoroutine(DamageFlashRoutine());
        }
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

    IEnumerator DamageFlashRoutine()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        
        spriteRenderer.color = transparentColor;
        yield return new WaitForSeconds(_damageCooldown);
        spriteRenderer.color = originalColor;
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
        if(_isShieldActive)
            return;
        _isShieldActive = true;
        GameObject shieldInstance = Instantiate(_shieldPrefab, transform.position, quaternion.identity);
        shieldInstance.transform.parent = this.transform;
    }
}
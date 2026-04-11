using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class Enemy : MonoBehaviour {
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private float _enemyHP = 1.0f;
    private int count = 0;
    private Laser laser;
    private void Start()
    {
        //starting from 5 seconds into the game, the method will be called every 30 seconds increasing the HP of the enemy by 1.0f, this can be changed and is not final
        InvokeRepeating(nameof(IncreaseHP),5f,30f);
    }
    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -8f)
        {
            transform.position = new Vector3(UnityEngine.Random.Range(-8,8),7 ,0);
            
        }
        if (count == 10)
        {
            laser.LaserDamage();
            count = 0;
        }
    }

    //other holds the information of the other obj that collided with enemy 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroy(GameObject.FindWithTag("Enemy"));
        }
        else if (other.tag == "Laser")
        {
            laser = other.GetComponent<Laser>();
            if (laser != null)
            {
                _enemyHP -= laser._laserDamage;
                if (_enemyHP <= 0)
                {
                    // Award points to the player for destroying the enemy
                    Score.Instance.AddScore();
                    
                    Destroy(other.gameObject);
                    Destroy(GameObject.FindWithTag("Enemy"));
                    count++;
                }
            }
        }
    }

    private void IncreaseHP()
    {
        _enemyHP += 1.0f;
    }
}

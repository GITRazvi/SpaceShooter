using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class Enemy : MonoBehaviour {
    [SerializeField]
    private float _speed = 4.0f;
    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -8f)
        {
            transform.position = new Vector3(UnityEngine.Random.Range(-8,8),7 ,0);
            
        }
    }

    //other hold the information of the other obj that collided with enemy 
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
            Destroy(GameObject.FindWithTag("Laser"));
            Destroy(GameObject.FindWithTag("Enemy"));

            /*
             Destroy(other.gameObject);
             Destroy(this.gameObject);
             */

        }
    }
}

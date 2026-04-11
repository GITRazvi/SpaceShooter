using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Laser : MonoBehaviour {

    [SerializeField]
    private float _speed = 8.0f;


    public float _laserDamage = 1.0f;

    private void Update()
    {
        transform.Translate(Vector3.up*_speed*Time.deltaTime);
        if (transform.position.y > 8f)
        {
            if(transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void LaserDamage()
    {
      _laserDamage += 1.0f;
    }
}

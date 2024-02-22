using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyCollision: MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private bool destroyOnCollision = false;
    [SerializeField] private bool isHazard = false;
    [SerializeField] private float bounceForce = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {   // Check collision on ground to destroy snowball
     
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Grab the PlayeHealth script and call the TakeDamage 
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        //Destroy the gameObject
        if (destroyOnCollision)
            Destroy(gameObject);

    }
}

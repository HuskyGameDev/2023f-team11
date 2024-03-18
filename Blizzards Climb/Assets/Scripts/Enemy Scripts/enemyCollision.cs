using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

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

            if (isHazard)
            {
                
                BlizzardMovement playerMovement = collision.gameObject.GetComponent<BlizzardMovement>();
                Vector2 lastInputDirection = playerMovement.LastInputDirection;
                

                // Check if there was any input (to avoid bouncing when standing still)
                //Debug.Log(lastInputDirection);
                if (lastInputDirection != Vector2.zero)
                {
                    //Debug.Log("penis");
                    // Apply the bounce force in the opposite direction of the last input
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 bounceDirection = -lastInputDirection.normalized; // Normalize to ensure consistent force
                        //Debug.Log("Bounce direction: " + bounceDirection);
                        playerRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
                    }
                } else if(lastInputDirection == Vector2.zero)
                {
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 bounceDirection = new Vector2(0,1); // Normalize to ensure consistent force
                        playerRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
        //Destroy the gameObject
        if (destroyOnCollision)
            Destroy(gameObject);

    }
}

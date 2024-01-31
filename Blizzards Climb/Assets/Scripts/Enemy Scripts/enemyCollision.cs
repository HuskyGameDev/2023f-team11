using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowballCollision : MonoBehaviour
{
    [SerializeField] private int damage = 1;
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
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject, .25f);
        }
        
    }
    
}

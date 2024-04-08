using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damage = 1;
    public bool destroyOnEvent = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            playerHealth.TakeDamage(damage);
        }
        if (destroyOnEvent == true)
        {
            // Destroy(collision.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player") {
            playerHealth.TakeDamage(damage);
        }
        if (destroyOnEvent == true)
        {
            // Destroy(collider.gameObject);
        }

    }
}

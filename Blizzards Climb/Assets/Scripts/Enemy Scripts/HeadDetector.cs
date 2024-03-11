using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDetector : MonoBehaviour
{
    GameObject Enemy;
    [SerializeField] GameObject respawnPoint;
    void Start()
    {
        Enemy = gameObject.transform.parent.gameObject; // intitializes the Enemy as the enemy game object
        if (respawnPoint == null)
        {
            respawnPoint = GameObject.Find("Smart Bird Startpoint");
        }
    }
    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<BlizzardMovement>(out var bliz)) // did the player cause the collision?
        {
            //collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(-1);
            bliz.rb.AddForce(Vector2.up * bliz.jumpingPower, ForceMode2D.Impulse); // add some feedback to killing an enemy
            respawnPoint.GetComponent<EnemyRespawn>().isDead = true;
        }
        Destroy(Enemy); // clean up enemy
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierSpawn : MonoBehaviour
{
    public GameObject enemy; // Sets the game object to the skier
    private GameObject spawnedEnemy; // The spawned enemy
    public float interval = 5; // Seconds to wait before spawn
    private float counter = 0; // Time since last spawn
    public float deathInterval = 10; // Time unitl death of enemy

    // Update is called once per frame
    void Update()
    {
        if(!spawnedEnemy && counter >= interval) {
            counter = 0;
            spawnedEnemy = Instantiate(enemy, transform.position, transform.rotation);
        }
        else if(spawnedEnemy && counter >= deathInterval) {
            Destroy(spawnedEnemy);
            counter = 0;
        }
        counter += Time.deltaTime;
    }

    

}

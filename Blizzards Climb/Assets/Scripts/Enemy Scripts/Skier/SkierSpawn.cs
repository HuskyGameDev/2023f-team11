using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierSpawn : MonoBehaviour
{
    public GameObject enemy; // Sets the game object to the skier
    public float interval = 250; // time interval between spawns
    private float counter = 0; // counter for number of skiers

    // Update is called once per frame
    void Update()
    {
        counter += 1;

        if(counter >= interval) {
            counter = 0;
            Instantiate(enemy, transform.position, transform.rotation);
        }
    }

}

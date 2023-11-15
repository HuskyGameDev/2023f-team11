using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastBirdSpawn : MonoBehaviour
{
    public GameObject bird; // Sets the game object to the bird
    public float interval = 250; // time interval between spawns
    private float counter = 0; // counter for number of birds

    // Update is called once per frame
    void Update()
    {
        counter += 1;

        if(counter >= interval) {
            counter = 0;
            Instantiate(bird, transform.position, transform.rotation);
        }
    }

    void OnBecameInvisible() 
    {
        Destroy(bird);
    }
}

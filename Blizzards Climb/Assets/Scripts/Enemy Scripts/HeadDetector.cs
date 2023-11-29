using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDetector : MonoBehaviour
{
    GameObject Enemy;

    void Start()
    {
        Enemy = gameObject.transform.parent.gameObject; // intitializes the Enemy as the enemy game object
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(Enemy);
    }
}

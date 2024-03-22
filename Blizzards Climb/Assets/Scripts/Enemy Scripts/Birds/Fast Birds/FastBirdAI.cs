using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastBirdAI : MonoBehaviour
{
    public GameObject startPoint; // Where the bird starts.
    private Rigidbody2D rb; // Bird's rigid body, what we use for movement.
    private Transform currentPoint;
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // sets rb to the Bird's rigidbody in Unity.
        currentPoint = startPoint.transform; // sets currentPoint to startPoint for initial starting position.
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = startPoint.transform.position - transform.position;
        if(currentPoint == startPoint.transform) {
            rb.velocity = new Vector2(-speed, 0);
        }
    }
}

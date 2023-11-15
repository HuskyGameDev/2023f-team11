using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierPatrol : MonoBehaviour
{
    public GameObject startPoint; // Where the skier starts.
    private Rigidbody2D rb; // Skier's rigid body, what we use for movement.
    private Animator anim; // Skier's animator.
    private Transform currentPoint;
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // sets rb to the Skier's rigidbody in Unity.
        anim = GetComponent<Animator>(); // sets anim to the Skier's Animator in Unity.
        currentPoint = startPoint.transform; // sets currentPoint to startPoint for initial starting position.
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = currentPoint.position - transform.position;
        if(currentPoint == startPoint.transform) {
            rb.velocity = new Vector2(0, -speed);
        }
    }

}

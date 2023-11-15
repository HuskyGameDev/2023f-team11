using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartBirdAI : MonoBehaviour
{
    public GameObject startPoint; // Where the bird starts.
    public GameObject endPoint; // Where the bird ends.
    private Rigidbody2D rb; // Bird's rigid body, what we use for movement.
    private Animator anim; // Bird's animator.
    private Transform currentPoint; // The point in which the bird will move towards.
    public float speed; // The speed at which the Bird will move.

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Sets rb to the Bird's rigidbody in Unity.
        anim = GetComponent<Animator>(); // Sets anim to the Bird's Animator in Unity.
        currentPoint = startPoint.transform; // Sets currentPoint to startPoint for initial starting position.
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = currentPoint.position - transform.position; // sets the direction for the Bird to move (Towards the currentPoint).

        if (currentPoint == startPoint.transform) // Checks the position of the Bird...
        { // This moves the bird right towards the starting point.
            rb.velocity = new Vector2(speed, 0); 
        } 
        else 
        { // This moves the bird left towards ending point.
            rb.velocity = new Vector2(-speed, 0); 
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == startPoint.transform)
        { // This will change the point destination of the Bird once it reaches the starting point.
            currentPoint = endPoint.transform;
            transform.localScale = new Vector3(1f, .5f, 1f); // This flips the sprite.
        } 

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == endPoint.transform)
        { // This will change the point destination of the Bird once it reaches the ending point.
            currentPoint = startPoint.transform;
            transform.localScale = new Vector3(-1f, .5f, 1f); // This flips the sprite.
        } 
    }

    // A method that makes the patrolling points easier to see in Unity.
    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(startPoint.transform.position, 0.5f);
        Gizmos.DrawWireSphere(endPoint.transform.position, 0.5f);
        Gizmos.DrawLine(startPoint.transform.position, endPoint.transform.position);
    }
}

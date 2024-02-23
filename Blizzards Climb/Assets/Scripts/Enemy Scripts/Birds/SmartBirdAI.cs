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
    private bool flipped = false;

    [Header("References")]
    [SerializeField, Tooltip("If no renderer is set then it will search the gameobject attached to the script for a renderer.")]
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float dropSnowballInterval = 5f;
    [SerializeField] public GameObject snowballPrefab;
    [SerializeField] private float snowballDropSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Sets rb to the Bird's rigidbody in Unity.
        anim = GetComponent<Animator>(); // Sets anim to the Bird's Animator in Unity.
        currentPoint = startPoint.transform; // Sets currentPoint to startPoint for initial starting position.

        // Use a coroutine for snowballs
        StartCoroutine(DropSnowballRoutine());
    }

    void awake()
    {
        if (!spriteRenderer) // sprite renderer is null
            spriteRenderer = GetComponent<SpriteRenderer>(); // find something
    }

    // Update is called once per frame
    void Update()
    {
        // never used? - Nathan
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
            Flip();
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == endPoint.transform)
        { // This will change the point destination of the Bird once it reaches the ending point.
            currentPoint = startPoint.transform;
            Flip();
        } 
    }

    //Coroutine for dropping snowballs
    // Using a Coroutine allows the script to do somethign simultaneously
    private IEnumerator DropSnowballRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dropSnowballInterval);
            DropSnowball();
        }

    }
    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    // Method for dropping snowball

    private void DropSnowball()
    {
        // Create an instance of the snowball at the birds position
        Vector2 spawnBelowBird = new Vector2(transform.position.x, transform.position.y - .5f);
        GameObject newSnowball = Instantiate(snowballPrefab, spawnBelowBird, Quaternion.identity);
        // Grab the rigidbody for the newSnowball
        Rigidbody2D snowballRB = newSnowball.GetComponent<Rigidbody2D>();
        // Check if it has a rigidbody, if so drop the snowball with the given snowBallDropSpeed
        if(snowballRB != null )
        {
            snowballRB.velocity = new Vector2(snowballRB.velocity.x, snowballDropSpeed * Time.deltaTime);
        }else
        {
            Debug.LogError("Rigidbody2D not found for the snowball");
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

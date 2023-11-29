using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlizzardMovement : MonoBehaviour
{
    private float horizontal; // player location of x axis
    private float speed = 8f; // speed of the player
    private float jumpingPower = 16f; // jumping power of the player
    private bool facingRight = true; // determines if player if facing left or right


    [SerializeField] private Rigidbody2D rb; // player's rigid body
    [SerializeField] private Transform groundCheck; // checks if player is on the ground
    [SerializeField] private LayerMask groundLayer; // the gorund layer 
    [SerializeField] private Transform iceCheck;
    [SerializeField] private LayerMask iceLayer;

   void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // instantiates the horizontal by getting which direction the player is moving
       
        if (Input.GetButtonDown("Jump") && IsGrounded())
        { // if the jump button is pushed and the player is grounded
        // the player's value on the y axis is set to the jumping power
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }


        if (IsGrounded() && IsIcey())
	    {
	        rb.velocity = new Vector2(4f, jumpingPower);
	    }

        if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
        { // if the player is in the air, this takes the player gradually 
        // back down once the jump button is released
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }


        Flip();
    }


    private void FixedUpdate()
    { // applies the speed and direction to the rigidbody of the player
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsIcey()
    {
        return Physics2D.OverlapCircle(iceCheck.position, 0.2f, iceLayer);
    } 
    
    private bool IsGrounded()
    { // checks to see if the player is standing on the ground by creating
        // an invisible circle at the players feet. When this circle touches
        // the ground layer, then the play can jump.
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    { // flips the sprite depending on which direction the player is facing.
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            facingRight = !facingRight; // player is facing left
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;


        }
    }

    /*private void OnCollisionEnter2D(Collision2D collider) {
         Debug.Log("Collision.");
         if (collider.gameObject.tag == "Enemy") {
             // Do damage
         }
    }*/
}


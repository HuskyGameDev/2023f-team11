using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlizzardMovement : MonoBehaviour
{
    private CustomInput playerInput; //Init the customInput Variable as input
    private float horizontalInput; // player location of x axis
    private bool facingRight = true; // determines if player if facing left or right
    private InputAction move = null; // InputAction for movement
    private InputAction jump = null; // InputAction for jumping
    private Vector2 moveDirection = Vector2.zero;

    [SerializeField] private float speed; // speed of the player
    [SerializeField] private float jumpingPower; // jumping power of the player
    [SerializeField] private Rigidbody2D player; // player's rigid body
    [SerializeField] private Transform groundCheck; // checks if player is on the ground
    [SerializeField] private LayerMask groundLayer; // the ground layer
    //[SerializeField] private Transform iceCheck;  // Checks if the player is on ice
    [SerializeField] private LayerMask iceLayer;  // Gets the iceLayer



    // Assigns the playerInput the customInput variable on wake
    private void Awake()
    {
        playerInput = new CustomInput();
    }

    private void OnEnable()
    {
        move = playerInput.Player.Movement;
        jump = playerInput.Player.Jump;
        move.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }


    void Update()
    {
        #region inputs
        moveDirection = move.ReadValue<Vector2>();
        horizontalInput = moveDirection.x;
        #endregion
        //New jump check uses the new input system to check if the button assigned to jump is pressed
        //And that the player is grounded
       
        if(jump.IsPressed() && IsGrounded())
        {
            Jump();
        }
        /* IsIcey is not yet properly implemented
        if (IsGrounded() && IsIcey())
	    {
	        player.velocity = new Vector2(4f, jumpingPower);
	    }
        */
        //Check which way the player is moving && facing and then call flip() to flip the sprite
        Flip();
    }

    private void Jump()
    {
        #region Do the Jump
        player.velocity = new Vector2(player.velocity.x, jumpingPower);
        #endregion
    }
    private void FixedUpdate()
    { // applies the speed and direction to the rigidbody of the player
        //Move the character 
        player.velocity = new Vector2(horizontalInput * speed * Time.deltaTime,player.velocity.y);
    }
    

    /* is not yet implemented
    private bool IsIcey()
    {
        return Physics2D.OverlapCircle(iceCheck.position, 0.2f, iceLayer);
    } 
    */
    private bool IsGrounded()
    { // checks to see if the player is standing on the ground by creating
        // an invisible circle at the players feet. When this circle touches
        // the ground layer, then the play can jump.
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    { // flips the sprite depending on which direction the player is facing.
        if (facingRight && horizontalInput < 0f || !facingRight && horizontalInput > 0f)
        {
            facingRight = !facingRight; // player is facing left
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;


        }
    }
    
    }
    /*private void OnCollisionEnter2D(Collision2D collider) {
         Debug.Log("Collision.");
         if (collider.gameObject.tag == "Enemy") {
             // Do damage
         }
    }*/


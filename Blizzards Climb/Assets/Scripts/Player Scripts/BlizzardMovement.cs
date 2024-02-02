using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlizzardMovement : MonoBehaviour
{
    private float horizontal; // player location of x axis
    private float speed = 8f; // speed of the player
    private float jumpingPower = 16f; // jumping power of the player
    private bool facingRight = true; // determines if player if facing left or right

    public Animator animator;//animation object for Blizzard

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
<<<<<<< Updated upstream
        return Physics2D.OverlapCircle(iceCheck.position, 0.2f, iceLayer);
    } 
    
    private bool IsGrounded()
=======
        // applies the speed and direction to the rigidbody of the player
        if (grounded && !onSlope())
            rb.AddForce(Vector2.right * input.x * speed, ForceMode2D.Force);
        else if (grounded && onSlope()) // else if you are on a slope and grounded move the rigidbody in the direction of slope.
            rb.AddForce(GetSlopeDirection(input) * speed * 1.5f, ForceMode2D.Force);
        else // else you are airborne use an airspeed (horizontal) multiplier instead of normal speed.
        {
            var _apexPoint = Mathf.InverseLerp(jumpingPower, 0, Mathf.Abs(rb.velocity.y));
            var _apexBonus = apexBonus * _apexPoint;
            rb.AddForce(Vector2.right * input.x * (airSpeed + _apexBonus), ForceMode2D.Force);
        }


        // lets you maintain some acceleration while keeping controls snappy.
        var deceleration = grounded ? groundDeceleration : airDeceleration;
        if (input.x == 0) rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime), rb.velocity.y);

        if (rb.velocity.x > 0 || rb.velocity.x < 0){
            animator.SetBool("Moving", true);
        }
        if (rb.velocity.x == 0){
            animator.SetBool("Moving", true);
        }
    }

    private void HandleJump()
    {
        // if you don't have the ability to jump (either you haven't pressed the jump button yet or you don't have a buffer jump ready
        // just exit the function early.
        if (!isJumping && !canBufferJump) return;
        //Debug.Log($"jump pressed? {jump} && canBufferJump? {canBufferJump}\nGrounded? {grounded} || canUseCoyote {canUseCoyote}"); // for some reason the player is able to jump a second time if they jump into a wall and press space bar again.

        // if you are grounded or can use the coyote jump then execute a jump.
        if (grounded || canUseCoyote) Jump();

        // you no longer want to jump after jumping.
        isJumping = false;
    }

    private void HandleFalling()
    {
        // if you are not grounded and your y velocity is positive
        if (!grounded && rb.velocity.y > 0)
        {
            // get the apex point of your jump.
            var _apexPoint = Mathf.InverseLerp(jumpingPower, 0, Mathf.Abs(rb.velocity.y));
            // change the gravity scale based on how close you are to the apex. (closer to apex means lesser gravity)
            rb.gravityScale = Mathf.Lerp(minGravity, cachedGravity, _apexPoint);
        }
        // only custom physics i'm going to do is clamping the fall speed of the player
        // we don't really want the player to have no control when falling (outside of maybe punishment for getting hit by an enemy?)
        // so clamp the speed at which the player falls giving them more time to react to the fall.
        else if (!grounded && rb.velocity.y < 0)
        {
            //Debug.Log($"velocity: {rb.velocity}");
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, clampedFallSpeed, 0));
        }
    }

    private void Jump()
    {
        // you can no longer use coyote after jumping.
        coyoteUsable = false;

        // can't buffer your jump while starting a jump.
        bufferJumpUsable = false;

        // don't want to use an old jump time so reset
        lastJumpPressed = 0;

        // reset the y velocity.
        rb.velocity = new Vector2(rb.velocity.x, 0);

        // add the jump force to the player
        rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
        //Debug.Log("Jumped!");
    }

    private Vector2 GetSlopeDirection(Vector2 direction)
    {
        // little bit of using 3d to remap to 2d.
        // basically just trying to project onto the plane that the slope makes from our directional inputs.
        // helps give the player a little push uphill
        return Vector3.ProjectOnPlane(direction, groundHit.normal).normalized;
    }

    private bool onSlope()
    {
        // calculate the angle between the normal from the ground and the up direction.
        var angle = Vector2.Angle(Vector2.up, groundHit.normal);
        // if the angle is less than an acceptable slope angle and not 0 then we are on a valid slope.
        return angle < maxSlopeAngle && angle != 0;
    }

    private void checkCollision()
>>>>>>> Stashed changes
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


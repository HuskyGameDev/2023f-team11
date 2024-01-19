using UnityEngine;

/// <summary>
/// much inspiration comes from tarodevs video on 2D platformer controller but instead of using custom physics it just uses rigidbody2D <br/>
/// https://www.youtube.com/watch?v=3sWTzMsmdx8
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class BlizzardMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f; // speed of the player
    [Tooltip("Deceleration takes place once the player stops giving horizontal inputs")]
    public float GroundDeceleration = 60;
    public float AirSpeed = 4f;
    [Tooltip("Deceleration takes place once the player stops giving horizontal inputs")]
    public float AirDeceleration = 120;

    private float horizontalInput; // input from player on horizontal axis. -1 for left +1 for right.


    [Header("Jumping")]
    public float jumpingPower = 16f; // jumping power of the player
    private bool jump;
    [Tooltip("What is the timeframe from pressing jump to landing can you buffer a jump (seconds)?")]
    public float jumpBuffer = .2f;
    private bool bufferJumpUsable;
    private bool canBufferJump => bufferJumpUsable && time < lastJumpPressed + jumpBuffer;

    [Tooltip("how long (seconds) after leaving an edge can you jump?")]
    public float CoyoteTime = .15f;
    private bool coyoteUsable; // can you jump after leaving solid ground?
    private bool canUseCoyote => coyoteUsable && !grounded && time < lastTimeOnGround + CoyoteTime; // are you able to use coyote time?

    private float lastJumpPressed;


    [Header("Ground Detection")]
    [SerializeField] private Vector2 GPosLocal;
    private Vector2 GPosGlobal => (Vector2)transform.position + GPosLocal;
    private bool grounded;
    [SerializeField] private Vector2 GPosSize;
    //[SerializeField] private float GroundCheckRadius;
    [SerializeField] private LayerMask groundLayer; // the gorund layer 
    private float lastTimeOnGround = float.MinValue; // when was the last time you touched the ground? 

    //[SerializeField] private Transform iceCheck;
    //[SerializeField] private LayerMask iceLayer;


    [Header("References")]
    [SerializeField, Tooltip("If no renderer is set then it will search the gameobject attached to the script for a renderer.")]
    private SpriteRenderer spriteRenderer;
    //private bool facingRight = true; // determines if player if facing left or right
    [SerializeField, Tooltip("If no Rigidbody2D is set then it will search the gameobject attached to the script for a Rigidbody2D.")]
    public Rigidbody2D rb; // player's rigid body

    // good for keeping track of how long it's been since something has happened or generally just to time buffers or jumps.
    private float time; // time since the controller has started.

    #region Player Controls (_PC)
    private CustomInput _PC; // use the new input system for player control so you can easily allow for interchange between arrow keys, wasd, or gamepad
    private void OnEnable()
    {
        _PC.Enable();
    }
    private void OnDisable()
    {
        _PC.Disable();
    }
    #endregion

    // Called when an enabled script instance is being loaded.
    private void Awake()
    {
        _PC = new CustomInput();

        if (!rb) // rigidbody is null
            rb = GetComponent<Rigidbody2D>(); // find something

        if (!spriteRenderer) // sprite renderer is null
            spriteRenderer = GetComponent<SpriteRenderer>(); // find something
    }

    private void GetInput()
    {
        // instantiates the horizontal by getting which direction the player is moving
        horizontalInput = _PC.Player.Movement.ReadValue<Vector2>().x;

        // check if player is trying to jump.
        if (_PC.Player.Jump.WasPressedThisFrame())
        {
            // now is the last time you pressed jump.
            lastJumpPressed = time;
            // you are trying to jump now.
            jump = true;
        }
    }

    void Update()
    {
        // What is trying to happen here?
        // I can only assume you are trying to induce slide to the player on ice.
        // But i believe you can just use physics materials to reduce friction on objects?
        // - Nathan
        //   if (IsGrounded() && IsIcey())
        //{
        //    rb.velocity = new Vector2(4f, jumpingPower);
        //}

        // What is trying to happen here?
        // if you are trying to simulate gravity the rigid body is already doing that.
        // also if you are trying to tell when the player has released the "jump" button it would be GetButtonUp("Jump")
        // - Nathan
        //if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
        //{ // if the player is in the air, this takes the player gradually 
        //  // back down once the jump button is released
        //  //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        //}

        time += Time.deltaTime; // get the time before anything is done because we are on a new frame.
        GetInput();

        // flip the player after getting input.
        Flip();
    }

    private void FixedUpdate()
    {
        // collision checks
        checkCollision();

        // handle external things like jumping.
        HandleJump();

        // move the player after everything else has been dealt with.
        MovePlayer();
    }

    private void MovePlayer()
    {
        // applies the speed and direction to the rigidbody of the player
        if (grounded)
            rb.AddForce(Vector2.right * horizontalInput * speed, ForceMode2D.Force);
        else // if you are airborne use an airspeed (horizontal) multiplier instead of normal speed.
            rb.AddForce(Vector2.right * horizontalInput * AirSpeed, ForceMode2D.Force);

        // lets you maintain some acceleration while keeping controls snappy.
        var deceleration = grounded ? GroundDeceleration : AirDeceleration;
        if (horizontalInput == 0) rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime), rb.velocity.y);
    }

    private void HandleJump()
    {
        // if you don't have the ability to jump (either you haven't pressed the jump button yet or you don't have a buffer jump ready
        // just exit the function early.
        if (!jump && !canBufferJump) return;
        //Debug.Log($"jump pressed? {jump} && canBufferJump? {canBufferJump}\nGrounded? {grounded} || canUseCoyote {canUseCoyote}"); // for some reason the player is able to jump a second time if they jump into a wall and press space bar again.

        // if you are grounded or can use the coyote jump then execute a jump.
        if (grounded || canUseCoyote) Jump();

        // you no longer want to jump after jumping.
        jump = false;
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
    }

    //private bool IsIcey()
    //{
    //    return Physics2D.OverlapCircle(iceCheck.position, 0.2f, iceLayer);
    //}

    private void checkCollision()
    { // checks to see if the player is standing on the ground by creating
      // an invisible circle at the players feet. When this circle touches
      // the ground layer, then the play can jump.

        // use a box cast to match the x dimension of the player bounding box to prevent player from getting stuck on edge of tilemap.
        bool groundHit = Physics2D.BoxCast(GPosGlobal, GPosSize, 0, Vector2.down, GPosSize.y, groundLayer);

        // if previous state was not on the ground but we detect ground
        if (!grounded && groundHit)
        {
            grounded = true; // we are grounded ;)
            bufferJumpUsable = true; // we can buffer a jump again.
            coyoteUsable = true; // and we can use coyote jump again.
        }
        // if we are previously grounded but no longer see ground
        else if (grounded && !groundHit)
        {
            grounded = false; // we are no longer grounded.
            lastTimeOnGround = time; // and the last time we saw the ground was now.
        }
        //return Physics2D.OverlapCircle(GPosGlobal, GroundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        // there is already an easy way to flip the direction of the player using the sprite renderer - Nathan
        // flips the sprite depending on which direction the player is facing.
        //if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        //{
        //    facingRight = !facingRight; // player is facing left
        //    Vector3 localScale = transform.localScale;
        //    localScale.x *= -1f;
        //    transform.localScale = localScale;
        //}

        // if looking left flip to look left.
        if (horizontalInput < 0) spriteRenderer.flipX = true;
        // else just look forward normally.
        // if we want it to look the direction that was last moved just change to "else if (horizontal > 0) spriteRenderer.flipX = false;"
        else spriteRenderer.flipX = false;
    }

    // when we select the gameobject attached to this script draw gizmos.
    private void OnDrawGizmosSelected()
    {
        // set the color so we can see the gizmo from the background.
        Gizmos.color = Color.red;
        // draw a cube to represent the ground position checker.
        Gizmos.DrawWireCube(GPosGlobal, GPosSize);
    }

    /*private void OnCollisionEnter2D(Collision2D collider) {
         Debug.Log("Collision.");
         if (collider.gameObject.tag == "Enemy") {
             // Do damage
         }
    }*/
}


using UnityEngine;

/// <summary>
/// much inspiration comes from tarodevs video on 2D platformer controller but instead of using custom physics it just uses rigidbody2D <br/>
/// https://www.youtube.com/watch?v=3sWTzMsmdx8
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class BlizzardMovement : MonoBehaviour
{
    [Header("Gravity")]
    public float clampedFallSpeed = -25f;
    private float minGravity => cachedGravity * .8f; // the minimum acceptable gravity is 80% of the cached gravity from Rigidbody Gravity Scale.
    private float cachedGravity; // caches the gravity from the Rigidbody Gravity scale


    [Header("Movement")]
    public float speed = 8f; // speed of the player
    [Tooltip("Deceleration takes place once the player stops giving horizontal inputs")]
    public float groundDeceleration = 60;
    public float airSpeed = 4f;
    [Tooltip("Deceleration takes place once the player stops giving horizontal inputs")]
    public float airDeceleration = 120;


    private Vector2 input; // input from player on the horizontal (x) axis -1 for left +1 for right.


    [Header("Jumping")]
    public float jumpingPower = 16f; // jumping power of the player
    public float apexBonus; // as you near the apex of your jump you should increase the horizontal speed of the player.
    private bool isJumping;
    [Tooltip("What is the timeframe from pressing jump to landing can you buffer a jump (seconds)?")]
    public float jumpBuffer = .2f;
    private bool bufferJumpUsable;
    private bool canBufferJump => bufferJumpUsable && time < lastJumpPressed + jumpBuffer;

    [Tooltip("how long (seconds) after leaving an edge can you jump?")]
    public float coyoteTime = .15f;
    private bool coyoteUsable; // can you jump after leaving solid ground?
    private bool canUseCoyote => coyoteUsable && !grounded && time < lastTimeOnGround + coyoteTime; // are you able to use coyote time?

    private float lastJumpPressed;


    [Header("Ground Detection")]
    [SerializeField] private Vector2 groundPositionLocal;
    private Vector2 groundPosGlobal => (Vector2)transform.position + groundPositionLocal;
    private bool grounded;
    [SerializeField] private Vector2 groundPosSize;
    //[SerializeField] private float GroundCheckRadius;
    [SerializeField] private LayerMask groundLayer; // the gorund layer 
    private float lastTimeOnGround = float.MinValue; // when was the last time you touched the ground? 


    // edge detection reuses the ground layer when looking for an edge to climb.
    [Header("Edge Detection")]
    [SerializeField] private Vector2 edgePositionLocal;
    private Vector2 edgePositionGlobal => (Vector2)transform.position + edgePositionLocal;
    private RaycastHit2D edgeHit;
    [SerializeField] private float edgeRayHit;
    public float edgeCorrectionPower = 4f;
    public float edgeGrabCooldown = .5f;
    private float lastEdgeGrab;
    private bool canEdgeGrab => edgeHit && input.x != 0 && time - edgeGrabCooldown > lastEdgeGrab;


    [Header("References")]
    [SerializeField, Tooltip("If no renderer is set then it will search the gameobject attached to the script for a renderer.")]
    private SpriteRenderer spriteRenderer;
    //private bool facingRight = true; // determines if player if facing left or right
    [SerializeField, Tooltip("If no Rigidbody2D is set then it will search the gameobject attached to the script for a Rigidbody2D.")]
    public Rigidbody2D rb; // player's rigid body

    // good for keeping track of how long it's been since something has happened or generally just to time buffers or jumps.
    private float time; // time since the controller has started.

    #region Player Controls (playerControls)
    private CustomInput playerControls; // use the new input system for player control so you can easily allow for interchange between arrow keys, wasd, or gamepad
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    #endregion

    // Called when an enabled script instance is being loaded.
    private void Awake()
    {
        playerControls = new CustomInput();


        if (!rb) // rigidbody is null
            rb = GetComponent<Rigidbody2D>(); // find something

        if (!spriteRenderer) // sprite renderer is null
            spriteRenderer = GetComponent<SpriteRenderer>(); // find something

        cachedGravity = rb.gravityScale;
    }

    private void GetInput()
    {
        // instantiates the horizontal by getting which direction the player is moving
        input = playerControls.Player.Movement.ReadValue<Vector2>();

        // check if player is trying to jump.
        if (playerControls.Player.Jump.WasPressedThisFrame())
        {
            //Debug.Log("Jump Pressed");
            // now is the last time you pressed jump.
            lastJumpPressed = time;
            // you are trying to jump now.
            isJumping = true;
        }
        //else if (_PC.Player.Jump.WasReleasedThisFrame()) Debug.Log("Jump Released");
    }

    void Update()
    {
        // messing with timescale to better see bugs.
        // if you need to change the timescale of the project when you see a bug just uncomment.
        //if (Input.GetKeyDown(KeyCode.Keypad0))
        //{
        //    Time.timeScale = 1;
        //}
        //else if (Input.GetKeyDown(KeyCode.Keypad1))
        //{
        //    Time.timeScale = .5f;
        //}
        //else if (Input.GetKeyDown(KeyCode.Keypad2))
        //{
        //    Time.timeScale = .2f;
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
        HandleFalling();

        // move the player after everything else has been dealt with.
        MovePlayer();
    }

    private void MovePlayer()
    {
        // applies the speed and direction to the rigidbody of the player
        if (grounded)
            rb.AddForce(Vector2.right * input.x * speed, ForceMode2D.Force);
        else // if you are airborne use an airspeed (horizontal) multiplier instead of normal speed.
        {
            var _apexPoint = Mathf.InverseLerp(jumpingPower, 0, Mathf.Abs(rb.velocity.y));
            var _apexBonus = apexBonus * _apexPoint;
            rb.AddForce(Vector2.right * input.x * (airSpeed + _apexBonus), ForceMode2D.Force);
        }


        // lets you maintain some acceleration while keeping controls snappy.
        var deceleration = grounded ? groundDeceleration : airDeceleration;
        if (input.x == 0) rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime), rb.velocity.y);
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
        if (!grounded && rb.velocity.y > 0)
        {
            var _apexPoint = Mathf.InverseLerp(jumpingPower, 0, Mathf.Abs(rb.velocity.y));
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

    private void checkCollision()
    { // checks to see if the player is standing on the ground by creating
      // an invisible circle at the players feet. When this circle touches
      // the ground layer, then the play can jump.

        // use a box cast to match the x dimension of the player bounding box to prevent player from getting stuck on edge of tilemap.
        bool groundHit = Physics2D.BoxCast(groundPosGlobal, groundPosSize, 0, Vector2.down, groundPosSize.y, groundLayer);
        // using a boxCast to check edges in the direction the player is moving to determine if the player can "climb" a ledge or not.
        edgeHit = Physics2D.Raycast(edgePositionGlobal, Vector2.right * Mathf.Sign(input.x), edgeRayHit, groundLayer);


        //Debug.Log($"has enough time passed to edge grab again? {_time - EdgeGrabCooldown > _lastEdgeGrab} because {_time - EdgeGrabCooldown} > {_lastEdgeGrab}\ncan player edgeGrab? {canEdgeGrab}");
        if (canEdgeGrab)
        {
            // reset y velocity
            rb.velocity = new Vector2(rb.velocity.x, 0);
            // nudge the player upwards so they can get on the edge.
            rb.AddForce(Vector2.up * edgeCorrectionPower, ForceMode2D.Impulse);
            lastEdgeGrab = time;
        }

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
    }

    private void Flip()
    {

        // if looking left flip to look left.
        if (input.x < 0) spriteRenderer.flipX = true;
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
        Gizmos.DrawWireCube(groundPosGlobal, groundPosSize);
        // draw a line to represent the edge grab raycast.
        Gizmos.DrawLine(edgePositionGlobal, edgePositionGlobal + (Mathf.Sign(input.x) * Vector2.right) * edgeRayHit);
    }

    /*private void OnCollisionEnter2D(Collision2D collider) {
         Debug.Log("Collision.");
         if (collider.gameObject.tag == "Enemy") {
             // Do damage
         }
    }*/
}


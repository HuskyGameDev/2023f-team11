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
    private float _minGravity => _cachedGravity * .8f; // the minimum acceptable gravity is 80% of the cached gravity from Rigidbody Gravity Scale.
    private float _cachedGravity; // caches the gravity from the Rigidbody Gravity scale


    [Header("Movement")]
    public float speed = 8f; // speed of the player
    [Tooltip("Deceleration takes place once the player stops giving horizontal inputs")]
    public float GroundDeceleration = 60;
    public float AirSpeed = 4f;
    [Tooltip("Deceleration takes place once the player stops giving horizontal inputs")]
    public float AirDeceleration = 120;


    private Vector2 _Input; // input from player on the horizontal (x) axis -1 for left +1 for right.


    [Header("Jumping")]
    public float jumpingPower = 16f; // jumping power of the player
    public float ApexBonus; // as you near the apex of your jump you should increase the horizontal speed of the player.
    private bool _jump;
    [Tooltip("What is the timeframe from pressing jump to landing can you buffer a jump (seconds)?")]
    public float jumpBuffer = .2f;
    private bool _bufferJumpUsable;
    private bool _canBufferJump => _bufferJumpUsable && _time < _lastJumpPressed + jumpBuffer;

    [Tooltip("how long (seconds) after leaving an edge can you jump?")]
    public float CoyoteTime = .15f;
    private bool _coyoteUsable; // can you jump after leaving solid ground?
    private bool _canUseCoyote => _coyoteUsable && !_grounded && _time < _lastTimeOnGround + CoyoteTime; // are you able to use coyote time?

    private float _lastJumpPressed;


    [Header("Ground Detection")]
    [SerializeField] private Vector2 GPosLocal;
    private Vector2 _GPosGlobal => (Vector2)transform.position + GPosLocal;
    private bool _grounded;
    [SerializeField] private Vector2 GPosSize;
    //[SerializeField] private float GroundCheckRadius;
    [SerializeField] private LayerMask groundLayer; // the gorund layer 
    private float _lastTimeOnGround = float.MinValue; // when was the last time you touched the ground? 


    // edge detection reuses the ground layer when looking for an edge to climb.
    [Header("Edge Detection")]
    [SerializeField] private Vector2 EPosLocal;
    private Vector2 _EPosGlobal => (Vector2)transform.position + EPosLocal;
    private RaycastHit2D _edgeHit;
    [SerializeField] private float EdgeRayLength;
    public float EdgeCorrectionPower = 4f;
    public float EdgeGrabCooldown = .5f;
    private float _lastEdgeGrab;
    private bool canEdgeGrab => _edgeHit && _Input.x != 0 && _time - EdgeGrabCooldown > _lastEdgeGrab;


    [Header("References")]
    [SerializeField, Tooltip("If no renderer is set then it will search the gameobject attached to the script for a renderer.")]
    private SpriteRenderer spriteRenderer;
    //private bool facingRight = true; // determines if player if facing left or right
    [SerializeField, Tooltip("If no Rigidbody2D is set then it will search the gameobject attached to the script for a Rigidbody2D.")]
    public Rigidbody2D rb; // player's rigid body

    // good for keeping track of how long it's been since something has happened or generally just to time buffers or jumps.
    private float _time; // time since the controller has started.

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

        _cachedGravity = rb.gravityScale;
    }

    private void GetInput()
    {
        // instantiates the horizontal by getting which direction the player is moving
        _Input = _PC.Player.Movement.ReadValue<Vector2>();

        // check if player is trying to jump.
        if (_PC.Player.Jump.WasPressedThisFrame())
        {
            //Debug.Log("Jump Pressed");
            // now is the last time you pressed jump.
            _lastJumpPressed = _time;
            // you are trying to jump now.
            _jump = true;
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

        _time += Time.deltaTime; // get the time before anything is done because we are on a new frame.
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
        if (_grounded)
            rb.AddForce(Vector2.right * _Input.x * speed, ForceMode2D.Force);
        else // if you are airborne use an airspeed (horizontal) multiplier instead of normal speed.
        {
            var _apexPoint = Mathf.InverseLerp(jumpingPower, 0, Mathf.Abs(rb.velocity.y));
            var _apexBonus = ApexBonus * _apexPoint;
            rb.AddForce(Vector2.right * _Input.x * (AirSpeed + _apexBonus), ForceMode2D.Force);
        }


        // lets you maintain some acceleration while keeping controls snappy.
        var deceleration = _grounded ? GroundDeceleration : AirDeceleration;
        if (_Input.x == 0) rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime), rb.velocity.y);
    }

    private void HandleJump()
    {
        // if you don't have the ability to jump (either you haven't pressed the jump button yet or you don't have a buffer jump ready
        // just exit the function early.
        if (!_jump && !_canBufferJump) return;
        //Debug.Log($"jump pressed? {jump} && canBufferJump? {canBufferJump}\nGrounded? {grounded} || canUseCoyote {canUseCoyote}"); // for some reason the player is able to jump a second time if they jump into a wall and press space bar again.

        // if you are grounded or can use the coyote jump then execute a jump.
        if (_grounded || _canUseCoyote) Jump();

        // you no longer want to jump after jumping.
        _jump = false;
    }

    private void HandleFalling()
    {
        if (!_grounded && rb.velocity.y > 0)
        {
            var _apexPoint = Mathf.InverseLerp(jumpingPower, 0, Mathf.Abs(rb.velocity.y));
            rb.gravityScale = Mathf.Lerp(_minGravity, _cachedGravity, _apexPoint);
        }
        // only custom physics i'm going to do is clamping the fall speed of the player
        // we don't really want the player to have no control when falling (outside of maybe punishment for getting hit by an enemy?)
        // so clamp the speed at which the player falls giving them more time to react to the fall.
        else if (!_grounded && rb.velocity.y < 0)
        {
            //Debug.Log($"velocity: {rb.velocity}");
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, clampedFallSpeed, 0));
        }
    }

    private void Jump()
    {
        // you can no longer use coyote after jumping.
        _coyoteUsable = false;

        // can't buffer your jump while starting a jump.
        _bufferJumpUsable = false;

        // don't want to use an old jump time so reset
        _lastJumpPressed = 0;

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
        bool groundHit = Physics2D.BoxCast(_GPosGlobal, GPosSize, 0, Vector2.down, GPosSize.y, groundLayer);
        // using a boxCast to check edges in the direction the player is moving to determine if the player can "climb" a ledge or not.
        _edgeHit = Physics2D.Raycast(_EPosGlobal, Vector2.right * Mathf.Sign(_Input.x), EdgeRayLength, groundLayer);


        //Debug.Log($"has enough time passed to edge grab again? {_time - EdgeGrabCooldown > _lastEdgeGrab} because {_time - EdgeGrabCooldown} > {_lastEdgeGrab}\ncan player edgeGrab? {canEdgeGrab}");
        if (canEdgeGrab)
        {
            // reset y velocity
            rb.velocity = new Vector2(rb.velocity.x, 0);
            // nudge the player upwards so they can get on the edge.
            rb.AddForce(Vector2.up * EdgeCorrectionPower, ForceMode2D.Impulse);
            _lastEdgeGrab = _time;
        }

        // if previous state was not on the ground but we detect ground
        if (!_grounded && groundHit)
        {
            _grounded = true; // we are grounded ;)
            _bufferJumpUsable = true; // we can buffer a jump again.
            _coyoteUsable = true; // and we can use coyote jump again.
        }
        // if we are previously grounded but no longer see ground
        else if (_grounded && !groundHit)
        {
            _grounded = false; // we are no longer grounded.
            _lastTimeOnGround = _time; // and the last time we saw the ground was now.
        }
    }

    private void Flip()
    {

        // if looking left flip to look left.
        if (_Input.x < 0) spriteRenderer.flipX = true;
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
        Gizmos.DrawWireCube(_GPosGlobal, GPosSize);
        // draw a line to represent the edge grab raycast.
        Gizmos.DrawLine(_EPosGlobal, _EPosGlobal + (Mathf.Sign(_Input.x) * Vector2.right) * EdgeRayLength);
    }

    /*private void OnCollisionEnter2D(Collision2D collider) {
         Debug.Log("Collision.");
         if (collider.gameObject.tag == "Enemy") {
             // Do damage
         }
    }*/
}


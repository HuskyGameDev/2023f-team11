using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlizzardMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool facingRight = true;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform iceCheck;
    [SerializeField] private LayerMask iceLayer;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded() && IsIcey())
	    {
	        rb.velocity = new Vector2(4f, rb.velocity.y);
	    }
     
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }


        if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }


        Flip();
    }


    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsIcey()
    {
        return Physics2D.OverlapCircle(iceCheck.position, 0.2f, iceLayer);
    } 
    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }


    private void Flip()
    {
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;


        }
    }
}


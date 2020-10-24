using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ParticleSystem dust;
    private Collision coll;

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool isFacingRight = true;
    public float airMoveSpeed;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    public float jumpForce = 10f;
    public float jumpTimer;

    [Header("Gripping")]
    public bool isGripping;
    public bool isClimbing;

    [Header("Wall Sliding")]
    public bool isPushingWall;
    public bool isWallSliding;
    public float wallSlideSpeed;

    [Header("Wall Jumping")]
    public float wallJumpForce = 18f;
    public float wallJumpDirection = -1f;
    public Vector2 wallJumpVector;
    public Vector2 wallJumpAngle;
    public bool canJump;
    public bool isWallJumping;
    public bool hasWallJumped;

    [Header("Wall Climbing")]
    public float climbSpeed;

    [Header("Components")]
    public Rigidbody2D rb;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    

    [Header("Input Intents")]
    public bool jumpIntent;
    public bool grabIntent;

    [Header("Movement Input")]
    public float x;
    public float y;
    public float xRaw;
    public float yRaw;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        wallJumpAngle.Normalize();
    }


    // Update is called once per frame
    void Update()
    {
        GetPlayerMovement();
        
        // Allows for non frame perfect jumping.
        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
            
        }

        grabIntent = Input.GetButton("Fire1");
        jumpIntent = Input.GetButton("Jump");


    }

    void FixedUpdate()
    {

        MoveCharacter();

        
        // Handles Jump
        if (jumpTimer > Time.time && coll.isOnGround)
        {
            Jump();

        }
        
        if (coll.isTouchingWall)
        {
            if (jumpTimer > Time.time)
            {
                WallJump();
                Debug.Log("Wall jump reached");
            }
        }
        
            
        
        

       

        //WallClimb();
        //WallSlide();



        //WallJump();
        //ModifyPhysics();
    }

    public void ModifyPhysics()
    {
        bool isChangingDirection = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        #region Ground Physics
        if (coll.isOnGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || isChangingDirection)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        #endregion
        #region In-Air Physics
        else
        {
            
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }

        }
        #endregion
        
    }

    public void MoveCharacter()
    {
        /*if (isGrounded)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            
        } else if (!isGrounded && !isWallSliding && direction.x != 0f)
        {
            rb.AddForce(new Vector2(airMoveSpeed * direction.x, 0));
             if (Mathf.Abs(rb.velocity.x) > moveSpeed)
                {
                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
                }
        }*/

        // Ground movement
        
        if (coll.isOnGround)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        







        if (direction.x < 0 && isFacingRight)
        {
            Flip();
        } else if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        

    }

    public void Flip()
    {
        if (!isWallSliding)
        {
            if (coll.isOnGround)
            {
                CreateDust();
            }

            isFacingRight = !isFacingRight;
            wallJumpDirection *= -1;
            transform.Rotate(0, 180, 0);
        }
        
    }
    public void Jump()
    {   
            CreateDust();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * moveSpeed;
            jumpTimer = 0;

    }

    public void CreateDust()
    {
        dust.Play();
    }
     
    public void GetPlayerMovement()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
        direction = new Vector2(x, y);
    }

    

    public void WallSlide()
    {
        isPushingWall = false;
        if ((isFacingRight && coll.isTouchingWall && direction.x > 0) || (!isFacingRight && coll.isTouchingWall && direction.x < 0))
        {
            isPushingWall = true;
        }

        if (isPushingWall && !coll.isOnGround && rb.velocity.y < 0 && !isClimbing)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
        }
    }

    public void WallJump()
    {
        
        if (coll.isOnRightWall)
        {
            wallJumpVector = Vector2.left;
        }
        else
        {
            wallJumpVector = Vector2.right;
        }

        rb.velocity = Vector2.zero;
        rb.velocity = (Vector2.up / 1.5f + wallJumpVector / 1.5f) * jumpForce;
        hasWallJumped = true;
        jumpTimer = 0;
        //rb.AddForce(new Vector2(wallJumpForce * wallJumpAngle.x * wallJumpDirection, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
            
    }

    public void WallClimb()
    {

        if (grabIntent && coll.isTouchingWall && !coll.isOnGround)
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(0f, direction.y * climbSpeed);
        }
        else
        {
            // Restores gravity after releasing grab button.
            rb.gravityScale = gravity;
        }   
    }

    

}

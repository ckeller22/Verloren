using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ParticleSystem dust;

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool isFacingRight = true;
    public float airMoveSpeed;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    public float jumpForce = 10f;
    private float jumpTimer;

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
    public Vector2 wallJumpAngle;
    public bool canJump;

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
    public bool isGrounded = true;
    public bool isTouchingWall;
    public LayerMask groundMask;
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    public Transform wallCheckPoint;
    public Vector2 wallCheckSize;

    [Header("Input Intents")]
    public bool jumpIntent;
    public bool grabIntent;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpAngle.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerMovement();
        DetectCollisions();
        
        // Allows for non frame perfect jumping.
        if (Input.GetButtonDown("Jump") && (isGrounded || isTouchingWall || isWallSliding))
        {
            jumpTimer = Time.time + jumpDelay;
            
        }

        grabIntent = Input.GetButton("Fire1");
        


    }

    void FixedUpdate()
    {

        MoveCharacter();

        Jump();

        WallClimb();
        WallSlide();



        WallJump();
        //ModifyPhysics();
    }

    public void ModifyPhysics()
    {
        bool isChangingDirection = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        #region Ground Physics
        if (isGrounded)
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
        if ((isTouchingWall && isFacingRight && direction.x > 0) || (isTouchingWall && !isFacingRight && direction.x < 0))
        {
            return;
        }
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);







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
            if (isGrounded)
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
        if (jumpTimer > Time.time && isGrounded)
        {
            CreateDust();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * jumpForce;
            jumpTimer = 0;
        }
        

    }

    public void CreateDust()
    {
        dust.Play();
    }

    public void GetPlayerMovement()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void DetectCollisions()
    {
        // Ground dectection, draws an invisible box at base of player, checks for ground overlap, and sets isGrounded to true if overlap is found.
        isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundMask);
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, groundMask);
    }

    public void WallSlide()
    {
        isPushingWall = false;
        if ((isFacingRight && isTouchingWall && direction.x > 0) || (!isFacingRight && isTouchingWall && direction.x < 0))
        {
            isPushingWall = true;
        }

        if (isPushingWall && !isGrounded && rb.velocity.y < 0 && !isClimbing)
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
        if (jumpTimer > Time.time && (isTouchingWall || isWallSliding))
        {
            rb.AddForce(new Vector2(wallJumpForce * wallJumpAngle.x * wallJumpDirection, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
            canJump = false;
            jumpTimer = 0;
        }
        
            
    }

    public void WallClimb()
    {

        if (grabIntent && isTouchingWall && !isGrounded)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);
       
    }

}

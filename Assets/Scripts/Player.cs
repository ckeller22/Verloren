using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ParticleSystem dust;

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool isFacingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Gripping")]
    public bool isGripping;
    public bool isClimbing;

    [Header("Wall Sliding")]
    public bool isWallSliding;
    public float wallSlideSpeed;

    [Header("Wall Jumping")]
    public float wallJumpForce = 18f;
    public float wallJumpDirection = -1f;
    public Vector2 wallJumpAngle;
    public bool canJump;

    [Header("Components")]
    public Rigidbody2D rigidBody2D;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool isGrounded;
    public bool isTouchingWall;
    public LayerMask groundMask;
    

    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        wallJumpAngle.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        DetectCollisions();


        // Allows for non frame perfect jumping.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpTimer = Time.time + jumpDelay;
            
        }

        GetPlayerMovement();
    }

    void FixedUpdate()
    {
        
        MoveCharacter(direction.x);
        
        if(jumpTimer > Time.time && isGrounded)
        {
            Jump();
        }

        WallSlide();
        WallClimbing();
        WallJump();
        ModifyPhysics();
    }

    public void ModifyPhysics()
    {
        bool isChangingDirection = (direction.x > 0 && rigidBody2D.velocity.x < 0) || (direction.x < 0 && rigidBody2D.velocity.x > 0);

        #region Ground Physics
        if (isGrounded)
        {
            if (Mathf.Abs(direction.x) < 0.4f || isChangingDirection)
            {
                rigidBody2D.drag = linearDrag;
            }
            else
            {
                rigidBody2D.drag = 0f;
            }
            rigidBody2D.gravityScale = 0;
        }
        #endregion
        #region In-Air Physics
        else
        {
            rigidBody2D.gravityScale = gravity;
            rigidBody2D.drag = linearDrag * 0.15f;
            if (rigidBody2D.velocity.y < 0)
            {
                rigidBody2D.gravityScale = gravity * fallMultiplier;
            }
            else if(rigidBody2D.velocity.y > 0 && !Input.GetButton("Jump")) {
                rigidBody2D.gravityScale = gravity * (fallMultiplier / 2);
            }
            
        }
        #endregion
        
    }

    public void MoveCharacter(float horizontal)
    {
        rigidBody2D.AddForce(Vector2.right * horizontal * moveSpeed);
        if(Mathf.Abs(rigidBody2D.velocity.x) > maxSpeed)
        {
            rigidBody2D.velocity = new Vector2(Mathf.Sign(rigidBody2D.velocity.x) * maxSpeed, rigidBody2D.velocity.y);
        }

        if ((horizontal > 0 && !isFacingRight) || ( horizontal < 0 && isFacingRight)) {
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
            transform.Rotate(0, isFacingRight ? 0 : 180, 0);
        }
        
    }
    public void Jump()
    {
        CreateDust();
        canJump = true;
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
        rigidBody2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
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
        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f), new Vector2(0.9f, 0.4f), 0f, groundMask);
        isTouchingWall = Physics2D.OverlapBox(new Vector2(isFacingRight ? gameObject.transform.position.x + 0.1f : gameObject.transform.position.x - 0.1f, gameObject.transform.position.y), new Vector2(0.9f, 0.4f), 0f, groundMask);
    }

    public void WallSlide()
    {
        if (isTouchingWall && !isGrounded && rigidBody2D.velocity.y < 0 && !isClimbing)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, wallSlideSpeed);
        }
    }

    public void WallJump()
    {
        if ((isWallSliding || isTouchingWall) && Input.GetButtonDown("Jump"))
        {
            rigidBody2D.AddForce(new Vector2(wallJumpForce * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
            canJump = false;
        }
    }

    public void WallClimbing()
    {
        if (Input.GetButton("Fire2") && isTouchingWall)
        {
            isClimbing = true;
        } 
        else
        {
            isClimbing = false;
        }
        if (isClimbing)
        {
            rigidBody2D.gravityScale = 0f;
            rigidBody2D.velocity = Vector2.zero;
            

        }
    }
}

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

    [Header("Components")]
    public Rigidbody2D rigidBody2D;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool isGrounded;
    public LayerMask groundMask;
    

    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ground dectection, draws an invisible box at base of player, checks for ground overlap, and sets isGrounded to true if overlap is found.
        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f), new Vector2(0.9f, 0.4f), 0f, groundMask);
        
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpTimer = Time.time + jumpDelay;
            
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    
    }

    void FixedUpdate()
    {
        
        MoveCharacter(direction.x);
        
        if(jumpTimer > Time.time && isGrounded)
        {
            Jump();
        }

        ModifyPhysics();
    }

    public void ModifyPhysics()
    {
        bool isChangingDirection = (direction.x > 0 && rigidBody2D.velocity.x < 0) || (direction.x < 0 && rigidBody2D.velocity.x > 0);

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
        CreateDust();
        isFacingRight = !isFacingRight;
        //transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
    }
    public void Jump()
    {
        CreateDust();
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
        rigidBody2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    public void CreateDust()
    {
        dust.Play();
    }
}

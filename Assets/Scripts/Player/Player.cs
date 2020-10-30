using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimb LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    #endregion

    #region Components
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Collision Collision { get; private set; }
    #endregion

    #region Check Transforms

    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;

    #endregion

    #region Other Variables
    [SerializeField]
    private PlayerData playerData;

    public int FacingDirection { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }

    private Vector2 workspace;

    
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {   
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, playerData, StateMachine);
        MoveState = new PlayerMoveState(this, playerData, StateMachine);
        JumpState = new PlayerJumpState(this, playerData, StateMachine);
        InAirState = new PlayerInAirState(this, playerData, StateMachine);
        LandState = new PlayerLandState(this, playerData, StateMachine);
        WallClimbState = new PlayerWallClimbState(this, playerData, StateMachine);
        WallGrabState = new PlayerWallGrabState(this, playerData, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, playerData, StateMachine);
        WallJumpState = new PlayerWallJumpState(this, playerData, StateMachine);
        LedgeClimbState = new PlayerLedgeClimb(this, playerData, StateMachine);
        DashState = new PlayerDashState(this, playerData, StateMachine);
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        InputHandler = GetComponent<PlayerInputHandler>();
        Collision = GetComponent<Collision>();

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Setters
    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;

    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        workspace = direction * velocity;
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityZero()
    {
        RB.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }
    #endregion

    #region Checks

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingWallBack()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if(xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    #endregion

    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
        
    }

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);

        float xDist = xHit.distance;
        workspace.Set(xDist * FacingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)(workspace), Vector2.down, ledgeCheck.position.y - wallCheck.position.y, playerData.whatIsGround);
        float yDist = yHit.distance;

        workspace.Set(wallCheck.position.x + (xDist * FacingDirection), ledgeCheck.position.y - yDist);
        return workspace;
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    #endregion

    #region Components
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Collision Collision { get; private set; }
    #endregion

    #region Other Variables
    [SerializeField]
    private PlayerData playerData;
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
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        InputHandler = GetComponent<PlayerInputHandler>();
        Collision = GetComponent<Collision>();
        
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

    

    public bool CheckIfGrounded()
    {
        return Collision.isGrounded;
    }
}
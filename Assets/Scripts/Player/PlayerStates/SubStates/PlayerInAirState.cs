using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class PlayerInAirState : PlayerState
{
    private bool isGrounded;
    private int xInput;
    private bool isJumping;
    private bool jumpInputStop;
    private bool jumpInput;
    private bool isTouchingWall;
    private bool grabInput;
    private bool isTouchingWallBack;
    private bool coyoteTime;
    private bool wallJumpCoyoteTime;
    private float startWallJumpCoyoteTime;
    private bool oldIsTouchingWall;
    private bool oldIsTouchingWallBack;
    private bool isTouchingLedge;
    private bool dashInput;
    
    public PlayerInAirState(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine) : base(player, playerData, playerStateMachine)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        oldIsTouchingWall = isTouchingWall;
        oldIsTouchingWallBack = isTouchingWallBack;

        isGrounded = player.CheckIfGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
        isTouchingWallBack = player.CheckIfTouchingWallBack();
        isTouchingLedge = player.CheckIfTouchingLedge();

        if (isTouchingWall && !isTouchingLedge)
        {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }

        if (!wallJumpCoyoteTime && !isTouchingWall && !isTouchingWallBack && (oldIsTouchingWall || oldIsTouchingWallBack))
        {
            StartWallJumpCoyoteTime();
        }
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        oldIsTouchingWall = false;
        oldIsTouchingWallBack = false;
        isTouchingWall = false;
        isTouchingWallBack = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckCoyoteTime();
        CheckWallJumpCoyoteTime();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;

        player.CheckIfShouldFlip(xInput);
        CheckJumpMultiplier();


        if (dashInput && player.DashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (isTouchingWall && !isTouchingLedge && !isGrounded)
        {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
        else if (jumpInput && (isTouchingWall || isTouchingWallBack || wallJumpCoyoteTime))
        {
            StopWallJumpCoyoteTime();
            isTouchingWall = player.CheckIfTouchingWall();
            player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if (jumpInput && player.JumpState.CanJump())
        {
            
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isTouchingWall && grabInput && isTouchingLedge)
        {
            stateMachine.ChangeState(player.WallGrabState);
        }
        else if (isTouchingWall && xInput == player.FacingDirection)
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
        else
        {
            player.SetVelocityX(playerData.moveSpeed * xInput);
            
            
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void CheckJumpMultiplier()
    {
        if (isJumping)
        {
            if (jumpInputStop)
            {
                player.SetVelocityY(player.CurrentVelocity.y * playerData.variableJumpHeightMultiplier);
                isJumping = false;
                
            }
            else if (player.CurrentVelocity.y <= 0f)
            {
                
                isJumping = false;
            }
        }
    }

    private void CheckCoyoteTime()
    {
        if (coyoteTime && Time.time > startTime + playerData.coyoteTime)
        {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }


    public void StartCoyoteTime()
    {
        coyoteTime = true;
    }

    private void CheckWallJumpCoyoteTime()
    {
        if (wallJumpCoyoteTime && Time.time > startWallJumpCoyoteTime + playerData.wallJumpCoyoteTime)
        {
            wallJumpCoyoteTime = false;
        }
    }

    private void StartWallJumpCoyoteTime()
    {
        wallJumpCoyoteTime = true;
        startWallJumpCoyoteTime = Time.time;
    }

    private void StopWallJumpCoyoteTime()
    {
        wallJumpCoyoteTime = false;
    }

    public void SetIsJumping()
    {
        isJumping = true;
    }

}

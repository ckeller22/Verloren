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
    
    public PlayerInAirState(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine) : base(player, playerData, playerStateMachine)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;

        player.CheckIfShouldFlip(xInput);
        CheckJumpMultiplier();
        

        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (jumpInput && player.JumpState.CanJump())
        {   
            
            stateMachine.ChangeState(player.JumpState);
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

    


    public void SetIsJumping()
    {
        isJumping = true;
    }
}

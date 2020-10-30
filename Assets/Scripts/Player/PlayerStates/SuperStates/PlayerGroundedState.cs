using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 input;
    protected int xInput;

    private bool jumpInput;
    private bool grabInput;
    private bool dashInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingLedge;

    public PlayerGroundedState(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine) : base(player, playerData, playerStateMachine)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
        isTouchingLedge = player.CheckIfTouchingLedge();
    }

    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
        player.DashState.ResetCanDash();
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
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;

        if (!isGrounded)
        {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
        if (dashInput)
        {
            stateMachine.ChangeState(player.DashState);
        }
        if (jumpInput && player.JumpState.CanJump())
        {
            
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isTouchingWall && grabInput && isTouchingLedge)
        {
            stateMachine.ChangeState(player.WallGrabState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

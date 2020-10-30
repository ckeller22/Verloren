using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerLedgeClimb : PlayerState
{
    private Vector2 detectedPosition;
    private Vector2 cornerPosition;
    private Vector2 startPosition;
    private Vector2 stopPosition;
    private bool isHanging;
    private bool isClimbing;
    private bool jumpInput;

    private int xInput;
    private int yInput;
    public PlayerLedgeClimb(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine) : base(player, playerData, playerStateMachine)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        player.transform.position = detectedPosition;
        cornerPosition = player.DetermineCornerPosition();

        startPosition.Set(cornerPosition.x - (player.FacingDirection * playerData.startOffset.x), cornerPosition.y - playerData.startOffset.y);
        stopPosition.Set(cornerPosition.x - (player.FacingDirection * playerData.stopOffset.x), cornerPosition.y - playerData.stopOffset.y);

        player.transform.position = startPosition;
    }

    public override void Exit()
    {
        base.Exit();

        isHanging = false;

        if (isClimbing)
        {
            player.transform.position = stopPosition;
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;

        player.SetVelocityZero();
        player.transform.position = startPosition;

        isHanging = true;

        if (xInput == player.FacingDirection && isHanging && !isClimbing)
        {
            isClimbing = true;
            stateMachine.ChangeState(player.IdleState);

        }
        else if (yInput == -1 && isHanging && !isClimbing)
        {
            stateMachine.ChangeState(player.InAirState);
        }
        else if (jumpInput && !isClimbing)
        {
            player.WallJumpState.DetermineWallJumpDirection(true);
            stateMachine.ChangeState(player.WallJumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetDetectedPosition(Vector2 position)
    {
        detectedPosition = position;
    }
}

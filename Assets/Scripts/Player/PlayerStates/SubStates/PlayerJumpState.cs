using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    private int amountOfJumpsLeft;

    public PlayerJumpState(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine) : base(player, playerData, playerStateMachine)
    {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        player.InputHandler.UseJumpInput();
        player.SetVelocityY(playerData.jumpSpeed);
        isAbilityDone = true;
        amountOfJumpsLeft--;
        player.InAirState.SetIsJumping();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool CanJump()
    {
        if(amountOfJumpsLeft > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void ResetAmountOfJumpsLeft()
    {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }

    public void DecreaseAmountOfJumpsLeft()
    {
        amountOfJumpsLeft--;
    }
}

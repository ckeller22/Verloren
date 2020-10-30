using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{   
    public bool CanDash { get; private set; }
    private float lastDashTime;
    private bool isHolding;
    private Vector2 dashDirection;
    protected int xInput;
    protected int yInput;

    public PlayerDashState(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine) : base(player, playerData, playerStateMachine)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        CanDash = false;
        player.InputHandler.UseDashInput();

        
    }

    public override void Exit()
    {
        base.Exit();

        if(player.CurrentVelocity.y > 0)
        {
            player.SetVelocityY(player.CurrentVelocity.y * playerData.dashEndYMultipler);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) { return; }

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;

        if (xInput == 0 || yInput == 0)
        {
            dashDirection = Vector2.right * player.FacingDirection;
        }
        else
        {
            dashDirection.Set(xInput, yInput);
        }
        player.RB.drag = playerData.drag;
        player.SetVelocity(playerData.dashSpeed, dashDirection);
        if (Time.time >= startTime + playerData.dashTime)
        {
            player.RB.drag = 0f;
            isAbilityDone = true;
        }

        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool CheckIfCanDash()
    {
        return CanDash && Time.time >= lastDashTime + playerData.dashCooldown;
    }

    public void ResetCanDash()
    {
        CanDash = true;
    }
}

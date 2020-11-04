using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{   
    public bool CanDash { get; private set; }
    private float lastDashTime;
    private bool isHolding;
    private Vector2 dashDirection;
    private Vector2 dashDirectionInput;
    private bool isDashVelocitySet;
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

        isDashVelocitySet = false;
        CanDash = false;
        player.InputHandler.UseDashInput();
        dashDirectionInput = player.InputHandler.DashDirectionInput;


    }

    public override void Exit()
    {
        base.Exit();
        
        if(player.CurrentVelocity.y >= 0)
        {
            player.SetVelocityY(player.CurrentVelocity.y * playerData.dashEndYMultipler);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) { return; }

        

        //if (!isDashVelocitySet)
        //{
            
        //    isDashVelocitySet = true;
        //}

        if (dashDirectionInput == Vector2.zero)
        {
            dashDirection = Vector2.right * player.FacingDirection;
            dashDirection.Normalize();

        }
        else
        {
            dashDirection.Set(dashDirectionInput.x, dashDirectionInput.y);

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

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerData playerData;
    protected PlayerStateMachine stateMachine;
    protected bool isExitingState;
    protected float startTime;
    
    
    public PlayerState(Player player, PlayerData playerData, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerData = playerData;
        this.stateMachine = playerStateMachine;
    }

    public virtual void Enter()
    {
        DoChecks();

        startTime = Time.time;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }

    public bool JumpInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }

    [SerializeField]
    private float inputHoldTime = 0.2f;
    private float jumpInputStartTime;
    private float jumpTimer;
    private float jumpDelay = 0.25f;
    public bool JumpInputStop { get; private set; }

    private void Update()
    {
        CheckJumpInputHoldTime();
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpTimer = Time.time + jumpDelay;
        }

        

        if (context.canceled)
        {
            JumpInputStop = true;
        }
    }

    public void UseJumpInput()
    {
        
        JumpInput = false;
    }

    private void CheckJumpInputHoldTime()
    {

        if (jumpTimer > Time.time)
        {
            JumpInput = false;
            
        }
    }
}

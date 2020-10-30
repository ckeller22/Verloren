using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float moveSpeed = 10f;

    [Header("Jump State")]
    public float jumpSpeed = 20f;
    public int amountOfJumps = 1;

    [Header("In Air State")]
    public float variableJumpHeightMultiplier = 0.5f;
    public float coyoteTime = 0.2f;

    [Header("Wall Slide State")]
    public float wallSlideSpeed = 3f;

    [Header("Wall Climb State")]
    public float wallClimbSpeed = 3f;

    [Header("Wall Jump State")]
    public float wallJumpSpeed = 20f;
    public float wallJumpTime = 0.4f;
    public Vector2 wallJumpAngle = new Vector2(1, 2);
    public float wallJumpCoyoteTime = 0.2f;

    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;

    [Header("Dash State")]
    public float dashCooldown = 0.5f;
    public float dashSpeed = 30f;
    public float dashTime = 0.2f;
    public float drag = 10f;
    public float dashEndYMultipler = 0.2f;
    public float distBetweenAfterImages = 0.5f;

    [Header("Check Variables")]
    public float groundCheckRadius = .3f;
    public float wallCheckDistance = 0.5f;
    public LayerMask whatIsGround;
}

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

    [Header("Wall Slide State")]
    public float wallSlideSpeed = 3f;

    [Header("Check Variables")]
    public float groundCheckRadius = .3f;
    public float wallCheckDistance = 0.5f;
    public LayerMask whatIsGround;
}

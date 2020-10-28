using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Collision")]
    public bool isGrounded = true;
    public bool isTouchingWall;
    public bool isOnRightWall;
    public bool isOnLeftWall;
    public LayerMask groundMask;


    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetectCollisions();
    }

    public void DetectCollisions()
    {
        // Ground dectection, draws an invisible box at base of player, checks for ground overlap, and sets isGrounded to true if overlap is found.
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundMask);
        isTouchingWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundMask) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundMask);
        isOnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundMask);
        isOnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

    }
}
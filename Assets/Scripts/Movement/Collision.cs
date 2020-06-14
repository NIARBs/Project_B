using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask ememyLayer;

    [Space]

    public bool onGround;
    public bool onEnemy;
    public bool canJump;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public float bottomRay = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    [Space]
    
    [Header("GroundRemember")]

    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        onEnemy = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, ememyLayer);

        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        fGroundedRemember -= Time.deltaTime;
        if (onGround)
        {
            fGroundedRemember = fGroundedRememberTime;
        }

        if(fGroundedRemember > 0 && rb.velocity.y < 0)
        {
            onGround = true;
        }

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + bottomOffset, -1 * transform.up, bottomRay, groundLayer);
        if (hit && rb.velocity.y < 0)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        if(onLeftWall == true && rb.velocity.y > 0)
        {
            onLeftWall = false;
        }

        if (onRightWall == true && rb.velocity.y > 0)
        {
            onRightWall = false;
        }

        onWall = onRightWall || onLeftWall;

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Debug.DrawRay((Vector2)transform.position + bottomOffset, -1 * transform.up * bottomRay, Color.blue);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
}

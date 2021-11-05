using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public float maxSpeed = 5.0f;
    public float forceSpeed = 10.0f;
    public float jumpSpeed = 8.0f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool grounded = onGround();
        float wallDir = onWallDir();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (Mathf.Abs(horizontal) > 0.1f)
        {
            // Horizontal movement
            if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.AddForce(new Vector2(Mathf.Sign(horizontal) * forceSpeed, 0.0f));
            }
            else
            {
                // Limit speed to avoid force increase
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
            }
        }
        else
        {
            if (grounded && Mathf.Abs(rb.velocity.x) > 2)
            {
                // Moving on ground with no input
                // Reduce velocity to avoid slide
                rb.velocity = new Vector2(rb.velocity.x/2, 0);
            }
        }
        if (vertical > 0.1f && (grounded || wallDir != 0))
        {
            // Allow jump
            if(wallDir != 0.0f)
            {
                // Wall jump
                rb.velocity = new Vector2(wallDir * -5.0f, jumpSpeed);
            }
            else
            {
                // Ground jump
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }
        }
    }

    private bool onGround()
    {
        float maxDistance = GetComponent<Renderer>().bounds.extents.y + 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxDistance);
        return hit.collider != null;
    }

    private float onWallDir()
    {
        float maxDistance = GetComponent<Renderer>().bounds.extents.x + 0.1f;
        RaycastHit2D rightWall = Physics2D.Raycast(transform.position, Vector2.right, maxDistance);
        if (rightWall.collider != null)
        {
            return 1.0f;
        }
        RaycastHit2D leftWall = Physics2D.Raycast(transform.position, Vector2.left, maxDistance);
        if (leftWall.collider != null)
        {
            return -1.0f;
        }
        return 0.0f;
    }
}
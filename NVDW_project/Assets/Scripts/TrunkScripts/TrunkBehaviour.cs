using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TrunkBehaviour : MonoBehaviour
{
    [Header("Physics")]
    public float walkSpeed;
    public int hitsToDie = 2;
    public GameObject bullet;
    public Transform shotPoint;
    
    [Header("Custom Behaviour")] 
    public bool followEnabled = true;
    public bool directionLookEnabled = true;
    
    
    [Header("Ground & Walls check")]
    public Transform groundCheckPos;
    public Transform backGroundCheck;
    public LayerMask groundLayer;
    public Collider2D bodyCollider;
    

    [HideInInspector] public bool mustPatrol = true;
    [HideInInspector] public bool playerTooClose = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public bool inRange = false;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Animator anim;
    public bool isBouncing = false;
    private bool mustTurn;
    private bool inBorder;
    private Rigidbody2D rb;
    private float timeBtwShots;
    private bool flipState;

    // Start is called before the first frame update
    void Start()
    {
        target = null;
        mustPatrol = false;
        Flip();
        flipState = false;
        mustPatrol = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    /// <summary>
    /// Bounce the object's vertical velocity.
    /// </summary>
    /// <param name="value"></param>
    public void Bounce(float value)
    {
        rb.AddForce(Vector2.up*value);
    }

    /// <summary>
    /// Bounce the objects velocity in a direction.
    /// </summary>
    /// <param name="dir"></param>
    public void Bounce(Vector2 dir)
    {
        rb.AddForce(dir);
    }
    private void FixedUpdate()
    {
        if (mustPatrol && !isHit)
        {
            mustTurn = !Physics2D.OverlapCircle(groundCheckPos.position, 0.1f, groundLayer) || Physics2D.Raycast(transform.position, Vector3.right * transform.localScale.x, 0.7f, groundLayer);
        }

        if (inRange && playerTooClose && !isHit)
        {
            inBorder = !Physics2D.OverlapCircle(backGroundCheck.position, 0.1f, groundLayer);
        }

        // Attack if the user is in range
        if (inRange && followEnabled) Attack();

        // Patrol if the player is not detected
        if (mustPatrol && !isHit && !isBouncing) Patrol();

        if (rb.velocity.magnitude < 0.1f) isBouncing = false;
    }

    void Patrol()
    {
        //bodyCollider.IsTouchingLayers(groundLayer)
        if (mustTurn)
        {
            mustPatrol = false;
            Flip();
            flipState = !flipState;
            mustPatrol = true;
        }
        rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
        
    }

    void Attack()
    {
        float attackSpeed = Math.Abs(walkSpeed / 2f);

        //Direction calculation
        Vector2 direction = ((Vector2) target.transform.position - rb.position).normalized;
        Vector2 force = direction * attackSpeed;
        
        //Movement 
        // !bodyCollider.IsTouchingLayers(groundLayer)
        if (playerTooClose && !inBorder)
        {
            rb.velocity = new Vector2(-force.x, rb.velocity.y);
        }

        // Direction graphics handling
        if (directionLookEnabled)
        {
            // Rotation of the shotpoint
            if (flipState && direction.x < -0.0001f || !flipState && direction.x > 0.0001f)
            {
                flipState = !flipState;
                Flip();
            }
        }

    }
    
    void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        shotPoint.rotation *= Quaternion.Euler(0,180f,0);
        walkSpeed *= -1;
    }
    
    void TriggerShot()
    {
        Instantiate(bullet, shotPoint.position, shotPoint.rotation);
    }

    public void TriggerDeath()
    {
        Destroy(this.gameObject);
    }
    
    public void TriggerHit()
    {
        isHit = false;
        anim.SetBool("isHit", false);
    }
}

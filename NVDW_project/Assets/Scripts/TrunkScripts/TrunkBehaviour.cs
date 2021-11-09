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
    [HideInInspector] public Transform target;
    [HideInInspector] public Animator anim;
    
    private bool mustTurn;
    private bool inBorder;
    private Rigidbody2D rb;
    private float timeBtwShots;

    // Start is called before the first frame update
    void Start()
    {
        target = null;
        mustPatrol = false;
        Flip();
        mustPatrol = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (mustPatrol && !isHit)
        {
            mustTurn = !Physics2D.OverlapCircle(groundCheckPos.position, 0.1f, groundLayer) || Physics2D.Raycast(transform.position, Vector3.right * transform.localScale.x, 0.3f, groundLayer);
        }

        if (inRange && playerTooClose && !isHit)
        {
            inBorder = !Physics2D.OverlapCircle(backGroundCheck.position, 0.1f, groundLayer);
        }

        // Attack if the user is in range
        if (inRange && followEnabled) Attack();

        // Patrol if the player is not detected
        if (mustPatrol && !isHit) Patrol();
    }

    void Patrol()
    {
        if (mustTurn || bodyCollider.IsTouchingLayers(groundLayer))
        {
            mustPatrol = false;
            Flip();
            mustPatrol = true;
        }
        rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
        
    }

    void Attack()
    {
        float attackSpeed = walkSpeed / 2f;

        //Direction calculation
        Vector2 direction = ((Vector2) target.position - rb.position).normalized;
        Vector2 force = direction * attackSpeed;
        
        //Movement 
        if (playerTooClose && !inBorder && !bodyCollider.IsTouchingLayers(groundLayer))
        {
            rb.velocity = new Vector2(-force.x, rb.velocity.y);
        }

        // Direction graphics handling
        if (directionLookEnabled)
        {
            // Rotation of the shotpoint

            if (!((transform.localScale.x < 0 && shotPoint.transform.rotation.y == 1) 
                || (transform.localScale.x > 0 && shotPoint.transform.rotation.y == 0)))
            { 
                shotPoint.rotation *= Quaternion.Euler(0,180f,0);
            }

            // Rotation of the enemy
            if (force.x < 0.001f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }else if (force.x > -0.001f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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

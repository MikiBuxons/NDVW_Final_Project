using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RinoBehaviour : MonoBehaviour
{
    [Header("Pathfinding")] 
    public float pathUpdateSeconds = 0.5f;
    public Vector2 force;
    public Vector2 direction;
    [Header("Physics")]
    public float walkSpeed;
    public int hitsToDie = 2;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 2.0f;
    public float jumpCheckOffset = 0.2f;

    [Header("Custom Behaviour")] 
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    
    
    [Header("Ground & Walls check")]
    public Transform groundCheckPos;
    public LayerMask groundLayer;


    [HideInInspector] public bool mustPatrol = true;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public bool inRange = false;
    [HideInInspector] public Transform target;
    [HideInInspector] public Animator anim;
    
    private bool isGrounded;
    public bool mustTurn;
    private Rigidbody2D rb;

    private Path path;
    private int currentWaypoint = 0;
    private Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        target = null;
        Flip();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

    }

    private void FixedUpdate()
    {
        if (mustPatrol && !isHit)
        {
            Debug.DrawRay(transform.position, Vector3.right * transform.localScale.x * 0.4f, Color.red);
            mustTurn = !Physics2D.OverlapCircle(groundCheckPos.position, 0.1f, groundLayer) || Physics2D.Raycast(transform.position, Vector3.right*transform.localScale.x, 0.4f, groundLayer);
        }
      
        // set the variable to jump
        //Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, jumpCheckOffset) ) isGrounded = true;
        else isGrounded = false;
        
        // Attack if the user is in range
        if (inRange && target != null && followEnabled) Attack();
        
        if (mustPatrol && !isHit)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (mustTurn)
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
        
    }

    void Attack()
    {
        float runSpeed = Mathf.Abs(walkSpeed) * 14f;
        
        if (path == null) {return;}
        
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //Direction calculation
        direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        force = direction * runSpeed;
        
        //Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * jumpModifier * runSpeed);
            }
        }
        
        //Movement 
        //rb.velocity = new Vector2(runSpeed * Time.fixedDeltaTime, rb.velocity.y);
        rb.AddForce(force);
        
        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        
        // Direction graphics handling
        if (directionLookEnabled)
        {
            if (force.x > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }else if (force.x < -0.01f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void UpdatePath()
    {
        if (inRange && target != null && followEnabled && seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpeed *= -1;
        mustPatrol = true;
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

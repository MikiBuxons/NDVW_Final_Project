using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Object = System.Object;

public class SlimeBehaviour : MonoBehaviour
{
    public float walkSpeed;
    public int hitsToDie;

    [HideInInspector] public bool mustPatrol;
    public bool mustTurn;
    [HideInInspector] public bool isHit;
    private Animator anim;
    public bool isBouncing = false;
    private Rigidbody2D rb;
    public Transform groundCheckPos;
    public LayerMask groundLayer;
    public Collider2D bodyCollider;

    // Start is called before the first frame update
    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        Flip();
        rb = GetComponent<Rigidbody2D>();
        mustPatrol = true;
        isHit = false;
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
        rb.velocity=dir;
    }
    private void FixedUpdate()
    {
        if (mustPatrol && !isHit)
        {
            mustTurn = !Physics2D.OverlapCircle(groundCheckPos.position, 0.1f, groundLayer) || Physics2D.Raycast(transform.position, Vector3.right * transform.localScale.x, 0.3f, groundLayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mustPatrol && !isHit && !isBouncing)
        {
            Patrol();
        }
        if (rb.velocity.magnitude<0.1f)
            isBouncing = false;
    }

    void Patrol()
    {
        if (mustTurn || bodyCollider.IsTouchingLayers(groundLayer))
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpeed , rb.velocity.y);
        
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
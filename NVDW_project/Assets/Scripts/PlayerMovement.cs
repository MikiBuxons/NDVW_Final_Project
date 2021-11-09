using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private Animator animator;
    public Rigidbody2D Rigidbody2D;
    private float Horizontal;
    private bool Attack;
    private bool Grounded;
    public int hit2Die;
    
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set horizontal and attack to its key
        Horizontal = Input.GetAxisRaw("Horizontal");
        Attack = Input.GetKeyDown(KeyCode.Mouse0);
        
        // set the variable to jump
        //Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.5f) ) Grounded = true;
        else Grounded = false;
        
        //if attack then start attacking animation and walwing animation
        if (Attack) animator.SetBool("attacking", true);
        else animator.SetBool("attacking", false);
        
        animator.SetBool("walking", Horizontal != 0.0f && Physics2D.Raycast(transform.position, Vector3.down, 0.5f) );
        
        // movement left and right
        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);


        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (!animator.GetBool("attacking")) Rigidbody2D.velocity = new Vector2(Horizontal * speed, Rigidbody2D.velocity.y);
    }

    public void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * jumpForce);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            //hit2Die--;
            Rigidbody2D.AddForce(Vector2.up * Vector2.right * 50);
        }
    }
}

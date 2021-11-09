using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float distance;
    public LayerMask whatIsSolid;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isDestructed", false);
        Invoke("DestructionAnimation", lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.DrawRay(transform.position, transform.right * distance, Color.red);
        RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, transform.right, distance, whatIsSolid);
        if (hitinfo.collider != null && !hitinfo.collider.CompareTag("Untagged"))
        {
            Debug.Log(hitinfo.collider.tag);
            if (hitinfo.collider.CompareTag("Player"))
            {
                Debug.Log("Enemy must take damage");
            }
            DestructionAnimation();
        }
        
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public void DestructionAnimation()
    {
        anim.SetBool("isDestructed", true);
    }
    
    public void TriggerDestroy()
    {
        Destroy(this.gameObject);
    }
}

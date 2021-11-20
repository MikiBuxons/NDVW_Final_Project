using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class RinoHit : MonoBehaviour
{
    private RinoBehaviour enemyParent;
    private Animator anim;
    public Collider2D collider2d;
    public Bounds Bounds => collider2d.bounds;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<RinoBehaviour>();
        anim = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        
        if (trig.gameObject.CompareTag("Player") && !enemyParent.isHit)
        {
            var player = trig.gameObject.GetComponent<PlayerController>();
            var willHurtEnemy = player.Bounds.center.y >= Bounds.max.y;
            if (willHurtEnemy)
            {
                enemyParent.hitsToDie --;
                if (enemyParent.hitsToDie > 0)
                {
                    enemyParent.isHit = true;
                    anim.SetBool("isHit", true);
                }
                else
                {
                    enemyParent.isHit = true;
                    anim.SetBool("isDead", true);
                }
                player.Bounce(5);
            }
            else
            {
                enemyParent.Bounce(10*(enemyParent.transform.position-player.transform.position).normalized);
                player.Bounce(-5*(enemyParent.transform.position-player.transform.position).normalized);
                player.health.Decrement();
                player.animator.SetTrigger("hurt");
            }
            
        }
    }
}
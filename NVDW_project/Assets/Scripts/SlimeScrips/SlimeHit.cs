using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class SlimeHit : MonoBehaviour
{
    private SlimeBehaviour enemyParent;
    private Animator anim;
    public Collider2D collider2d;
    public Bounds Bounds => collider2d.bounds;
    public float enemyReward=1;
    public float damagePenalty=1;
    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<SlimeBehaviour>();
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
                    player.AddReward(enemyReward);
                    enemyParent.isHit = true;
                    anim.SetBool("isDead", true);
                }
                player.Bounce(5);
            }
            else if (player.vulnerable)
            {
                player.animator.SetTrigger("hurt");
                player.AddReward(damagePenalty);
                enemyParent.isBouncing = true;
                enemyParent.Bounce(3*(enemyParent.transform.position-player.transform.position).normalized);
                player.Bounce(-3*(enemyParent.transform.position-player.transform.position).normalized);
                player.health.Decrement();
                
            }
            
        }
    }
}


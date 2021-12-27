using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class TrunkHit : MonoBehaviour
{
    private TrunkBehaviour enemyParent;
    private Animator anim;
    public Collider2D collider2d;
    public Bounds Bounds => collider2d.bounds;
    public float enemyReward=10;
    public float damagePenalty=10;
    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<TrunkBehaviour>();
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
                    player.reward += enemyReward;
                    anim.SetBool("isDead", true);
                }
                player.Bounce(5);
            }
            else
            {
                player.animator.SetTrigger("hurt");
                player.reward -= damagePenalty;
                enemyParent.isBouncing = true;
                enemyParent.Bounce(3*(enemyParent.transform.position-player.transform.position).normalized);
                player.Bounce(-3*(enemyParent.transform.position-player.transform.position).normalized);
                player.health.Decrement();
                
            }
            
        }
    }
}

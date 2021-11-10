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
                    anim.SetBool("isDead", true);
                }
                player.Bounce(-player.velocity.y);
            }
            else
            {
                player.Bounce(-player.velocity);
            }
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDamage : MonoBehaviour
{
    private SlimeBehaviour enemyParent;
    private Animator anim;

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
        }
    }
}

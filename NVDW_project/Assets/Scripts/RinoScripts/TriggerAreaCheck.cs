using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    private RinoBehaviour enemyParent;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<RinoBehaviour>();
        anim = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            enemyParent.inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            enemyParent.inRange = false;
            enemyParent.mustPatrol = true;
            anim.SetBool("playerLocated", false);
            enemyParent.target = null;
        }
    }
}

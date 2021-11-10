using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RinoView : MonoBehaviour
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
            enemyParent.mustPatrol = false;
            enemyParent.target = collider.transform;
            anim.SetBool("playerLocated", true);
        }
    }
    
}

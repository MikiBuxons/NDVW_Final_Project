using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkView : MonoBehaviour
{
    private TrunkBehaviour enemyParent;
    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<TrunkBehaviour>();
        anim = GetComponentInParent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            enemyParent.mustPatrol = false;
            enemyParent.inRange = true;
            enemyParent.target = collider.transform;
            anim.SetBool("playerLocated", true);
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

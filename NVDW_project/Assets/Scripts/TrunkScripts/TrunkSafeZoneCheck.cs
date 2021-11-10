using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkSafeZoneCheck : MonoBehaviour
{
    private TrunkBehaviour enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<TrunkBehaviour>();
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            enemyParent.playerTooClose = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            enemyParent.playerTooClose = false;
        }
    }
}

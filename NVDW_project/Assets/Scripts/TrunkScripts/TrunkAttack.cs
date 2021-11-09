using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkAttack : MonoBehaviour
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
            Debug.Log("hurt to player");
            //apply damage to the player
            //TODO
            
            //jump back
            //TODO
            //enemyParent.rb.AddForce(Vector2.up * jumpForce);
        }
    }
}

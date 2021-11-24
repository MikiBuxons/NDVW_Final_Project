using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttack : MonoBehaviour
{
    private SlimeBehaviour enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponentInParent<SlimeBehaviour>();
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

using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
       if (collision.transform.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerController>().health.Decrement();
            collision.gameObject.GetComponent<PlayerController>().Bounce(3);
       }
   }
}

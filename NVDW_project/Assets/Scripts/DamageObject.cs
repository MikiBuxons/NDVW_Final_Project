using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platformer.Mechanics;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public float bounceStrength;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player.vulnerable)
            {
                player.animator.SetTrigger("hurt");
                player.Bounce(-bounceStrength * (transform.position - player.transform.position).normalized);
                player.health.Decrement();
            }
        }
    }
}
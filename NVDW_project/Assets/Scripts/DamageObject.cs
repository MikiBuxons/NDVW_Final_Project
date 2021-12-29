using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platformer.Mechanics;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public float bounceStrength;
    public float damagePenalty = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player.vulnerable)
            {
                player.animator.SetTrigger("hurt");
                player.reward -= damagePenalty;
                player.Bounce(-bounceStrength * (transform.position - player.transform.position).normalized);
                player.health.Decrement();
            }

            var Saw = gameObject.GetComponent<WaypointFollow>();
            if (Saw)
                Saw.hitPlayer = true;
        }
    }
}
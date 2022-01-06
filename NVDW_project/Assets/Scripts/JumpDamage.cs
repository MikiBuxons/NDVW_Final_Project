using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics;

public class JumpDamage : MonoBehaviour
{
    public Collider2D collider2D;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject destroyParticles;
    public float jumpForce = 2.5f;
    public int lifes = 1;
    public float enemyReward;
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.transform.CompareTag("Player")) {
            //collision.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.up * jumpForce);
            var player = collision.gameObject.GetComponent<PlayerController>();
            player.Bounce(3);
            lifes--;
            animator.Play("Hit");

            if (lifes == 0)
            {
                player.AddReward(enemyReward);
                destroyParticles.SetActive(true);
                spriteRenderer.enabled = false;
                Invoke("EnemyDie", 0.2f);
            }
        }
    }

    public void EnemyDie() {
        Destroy(gameObject);
    }

}

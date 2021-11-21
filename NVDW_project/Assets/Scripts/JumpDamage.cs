using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDamage : MonoBehaviour
{
    public Collider2D collider2D;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject destroyParticles;
    public float jumpForce = 2.5f;
    public int lifes = 2;

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.transform.CompareTag("Player")) {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.up * jumpForce);
            
            lifes--;
            animator.Play("Hit");

            if (lifes == 0) {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnemy : MonoBehaviour
{
   private float waitedTime;
   public float waitTimeToAttack = 3;
   public Animator animator;
   public GameObject bulletPrefab;
   public Transform lauchSpawnPoint;
   private bool inRange;

    private void Start() {
        waitedTime = waitTimeToAttack;
    }

    private void Update() {
        if (inRange) {
            if (waitedTime <= 0) {
                waitedTime = waitTimeToAttack;
                animator.Play("Attack");

            } else {
                waitedTime -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {   
           inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    public void LaunchBullet() {
        GameObject newBullet;

        newBullet = Instantiate(bulletPrefab, lauchSpawnPoint.position, lauchSpawnPoint.rotation);
    }
}

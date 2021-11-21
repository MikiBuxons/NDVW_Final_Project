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

    private void Start() {
        waitedTime = waitTimeToAttack;
    }

    private void Update() {
        if (waitedTime <= 0) {
            waitedTime = waitTimeToAttack;
            animator.Play("Attack");

        } else {
            waitedTime -= Time.deltaTime;
        }
    }


    public void LaunchBullet() {
        GameObject newBullet;

        newBullet = Instantiate(bulletPrefab, lauchSpawnPoint.position, lauchSpawnPoint.rotation);
    }
}

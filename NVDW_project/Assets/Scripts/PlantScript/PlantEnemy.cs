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
    private GameObject player;
    private bool flipState = false;

    private void Start()
    {
        waitedTime = waitTimeToAttack;
    }

    private void Update()
    {
        if (inRange)
        {
            Vector2 direction = ((Vector2) player.transform.position - (Vector2) transform.position).normalized;
            if (!flipState && direction.x < -0.0001f || flipState && direction.x > 0.0001f)
            {
                flipState = !flipState;
                Flip();
            }

            if (waitedTime <= 0)
            {
                waitedTime = waitTimeToAttack;
                animator.Play("Attack");
            }
            else
            {
                waitedTime -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            player = collider.gameObject;
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            player = null;
            inRange = false;
        }
    }

    void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        lauchSpawnPoint.rotation *= Quaternion.Euler(0, 180f, 0);
    }


    public void LaunchBullet()
    {
        GameObject newBullet;

        newBullet = Instantiate(bulletPrefab, lauchSpawnPoint.position, lauchSpawnPoint.rotation);
    }
}
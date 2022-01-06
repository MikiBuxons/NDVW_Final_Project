using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platformer.Mechanics;
using UnityEngine;
public class GoalController : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().animator.SetTrigger("victory");
            }
        }
    }
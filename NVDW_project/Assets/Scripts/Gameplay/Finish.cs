using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;

public class Finish : MonoBehaviour
{
    public float victoryReward=10000;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // CompleteLevel();
            var player = collision.gameObject.GetComponent<PlayerController>();
            player.reward += victoryReward;
            player.EndEpisode();
            // Schedule<PlayerDeath>();
        }
    }

    private void CompleteLevel() {
        SceneManager.LoadScene(0);
    }
}

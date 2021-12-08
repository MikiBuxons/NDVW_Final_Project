using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string levelName;
    private bool inStart = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            inStart = true;
        }
    }    

    void Update()
    {
        if (inStart) {
            SceneManager.LoadScene(levelName);
        }
    }
}

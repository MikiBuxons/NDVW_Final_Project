using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour, IPointerClickHandler
{
    private bool inStart = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        inStart = true;
    }

    void Update()
    {
        if (inStart) {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

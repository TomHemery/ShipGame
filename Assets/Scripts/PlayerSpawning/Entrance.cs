using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Entrance : MonoBehaviour
{
    public string targetScene;
    public Transform spawnPoint;

    private bool firstUpdate = true;
    private bool pause = false;

    private void Update()
    {
        if (firstUpdate) firstUpdate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pause && !firstUpdate && collision.gameObject.CompareTag("PlayerShip"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (firstUpdate) {
            pause = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (pause) pause = false;
    }
}

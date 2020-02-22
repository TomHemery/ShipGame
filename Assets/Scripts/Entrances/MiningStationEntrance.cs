using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiningStationEntrance : MonoBehaviour
{
    public string targetScene;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerShip"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}

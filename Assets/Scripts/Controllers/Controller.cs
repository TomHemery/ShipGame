using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.onSimPause.AddListener(OnSimPause);
        GameManager.onSimUnPause.AddListener(OnSimUnPause);
        if (GameManager.SimPaused) {
            OnSimPause();
        }
    }

    private void OnDisable()
    {
        GameManager.onSimPause.RemoveListener(OnSimPause);
        GameManager.onSimUnPause.RemoveListener(OnSimUnPause);
    }

    private void OnSimPause()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnSimUnPause() {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    }
}
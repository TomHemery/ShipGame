using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnSimPause.AddListener(OnSimPause);
        GameManager.OnSimUnPause.AddListener(OnSimUnPause);
        if (GameManager.SimPaused) {
            OnSimPause();
        }
    }

    private void OnDisable()
    {
        GameManager.OnSimPause.RemoveListener(OnSimPause);
        GameManager.OnSimUnPause.RemoveListener(OnSimUnPause);
    }

    private void OnSimPause()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnSimUnPause() {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    }
}
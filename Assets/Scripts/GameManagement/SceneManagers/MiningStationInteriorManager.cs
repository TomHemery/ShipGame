using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiningStationInteriorManager : MonoBehaviour
{

    public static MiningStationInteriorManager Instance { get; private set; } = null;

    private GameObject playerShip;
    private GameObject mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        playerShip.GetComponent<PlayerShipController>().enabled = false;
        playerShip.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        mainCamera.GetComponent<FollowTargetController>().enabled = false;
    }

    private void OnDestroy()
    {
        playerShip.GetComponent<PlayerShipController>().enabled = true;
        playerShip.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        mainCamera.GetComponent<FollowTargetController>().enabled = true;
        Instance = null;
    }
}

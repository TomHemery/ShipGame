using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnController : MonoBehaviour
{

    public float animatedExitTime = 0.5f;
    private float elapsedExitTime = 0.0f;

    [HideInInspector]
    public string sceneEntryPoint = "";
    [HideInInspector]
    public bool animateExit = false;

    private PlayerShipController playerShipController;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerShipController = GetComponent<PlayerShipController>();
    }

    private void Update()
    {
        if (animateExit && !GameManager.SimPaused) {
            elapsedExitTime += Time.deltaTime;

            if (elapsedExitTime > animatedExitTime)
            {
                elapsedExitTime = 0.0f;
                animateExit = false;
                playerShipController.RespondToInput = true;
                playerShipController.thrustMode = ShipController.ThrustMode.None;
            }
            else {
                playerShipController.thrustMode = ShipController.ThrustMode.Forward;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 up = new Vector3(0, 1, 0);

        if(sceneEntryPoint != "")
        {
            Transform target = GameObject.Find(sceneEntryPoint).transform;
            pos = target.position;
            up = target.up;

            playerShipController.desiredRotation = up;

            sceneEntryPoint = "";
        }

        if (animateExit) {
            playerShipController.RespondToInput = false;
            playerShipController.thrustMode = ShipController.ThrustMode.Forward;
        }
        
        transform.position = pos;
        transform.up = up;
    }

    public void GotoSpawnPoint() {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 up = new Vector3(0, 1, 0);
        if (sceneEntryPoint != "")
        {
            Transform target = GameObject.Find(sceneEntryPoint).transform;
            pos = target.position;
            up = target.up;

            playerShipController.desiredRotation = up;

            sceneEntryPoint = "";
        }
        if (animateExit)
        {
            playerShipController.RespondToInput = false;
            playerShipController.thrustMode = ShipController.ThrustMode.Forward;
        }
        transform.position = pos;
        transform.up = up;
    }
}

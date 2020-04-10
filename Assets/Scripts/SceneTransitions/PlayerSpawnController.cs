using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnController : MonoBehaviour
{

    public float animatedExitTime = 0.5f;
    private float elapsedExitTime = 0.0f;
    private bool animateSpawn = true;

    public Transform spawnPoint;

    private PlayerShipController playerShipController;
    private PauseAndShowUIOnCollide miningStationUITrigger;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerShipController = GetComponent<PlayerShipController>();
        miningStationUITrigger = GameObject.FindGameObjectWithTag("MiningStation").GetComponent<PauseAndShowUIOnCollide>();
    }

    private void Update()
    {
        if (animateSpawn && !GameManager.SimPaused) {
            elapsedExitTime += Time.deltaTime;

            if (elapsedExitTime > animatedExitTime)
            {
                elapsedExitTime = 0.0f;
                animateSpawn = false;
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
        GotoSpawnPoint();
    }

    public void GotoSpawnPoint() {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 up = new Vector3(0, 1, 0);

        pos = spawnPoint.position;
        up = spawnPoint.up;

        playerShipController.desiredRotation = up;

        animateSpawn = true;
        playerShipController.RespondToInput = false;
        playerShipController.thrustMode = ShipController.ThrustMode.Forward;
        transform.position = pos;
        transform.up = up;
        miningStationUITrigger.PauseBehaviourUntilCollisionExit();
    }
}

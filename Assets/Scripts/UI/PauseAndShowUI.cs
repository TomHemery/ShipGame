using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAndShowUI : MonoBehaviour
{
    public GameObject uiToShow;

    public bool overwritePlayerPos = false;
    public string playerSpawn;
    public bool animatePlayerSpawn;

    private GameObject playerShip;
    private GameObject mainCamera;

    protected virtual void Awake()
    {
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        uiToShow.SetActive(false);
    }

    public void PauseScene() {
        GameManager.PauseSim();
        uiToShow.SetActive(true);
    }

    public virtual void UnPauseScene() {
        uiToShow.SetActive(false);

        GameManager.UnPauseSim();

        PlayerSpawnController playerSceneTransBehaviour = playerShip.GetComponent<PlayerSpawnController>();
        if (overwritePlayerPos)
        {
            playerSceneTransBehaviour.sceneEntryPoint = playerSpawn;
            playerSceneTransBehaviour.animateExit = animatePlayerSpawn;
        }
        playerSceneTransBehaviour.GotoSpawnPoint();
    }
}

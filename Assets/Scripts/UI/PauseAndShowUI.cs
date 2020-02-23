using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAndShowUI : MonoBehaviour
{

    public List<GameObject> toDisable;
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
        GameObject g;
        for (int i = toDisable.Count - 1; i >= 0; i--)
        {
            g = toDisable[i];
            if (g != null) g.SetActive(false);
            else toDisable.Remove(g);
        }

        uiToShow.SetActive(true);

        playerShip.GetComponent<PlayerShipController>().enabled = false;
        playerShip.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        mainCamera.GetComponent<FollowTargetController>().enabled = false;
    }

    public virtual void UnPauseScene() {
        foreach (GameObject g in toDisable) g.SetActive(true);
        uiToShow.SetActive(false);

        playerShip.GetComponent<PlayerShipController>().enabled = true;
        playerShip.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        mainCamera.GetComponent<FollowTargetController>().enabled = true;

        PlayerSceneTransitionBehaviour playerSceneTransBehaviour = playerShip.GetComponent<PlayerSceneTransitionBehaviour>();
        if (overwritePlayerPos)
        {
            playerSceneTransBehaviour.sceneEntryPoint = playerSpawn;
            playerSceneTransBehaviour.animateExit = animatePlayerSpawn;
        }
        playerSceneTransBehaviour.OnUnpause();
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseAndShowUI : MonoBehaviour
{
    public GameObject uiToShow;

    public bool sendPlayerToSpawn = false;

    private GameObject playerShip;
    private GameObject mainCamera;

    public UnityEvent onShowUI = new UnityEvent();
    public UnityEvent onHideUI = new UnityEvent();

    protected virtual void Awake()
    {
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        uiToShow.SetActive(false);
    }

    public void PauseAndShow() {
        GameManager.PauseSim();
        uiToShow.SetActive(true);
        onShowUI.Invoke();
    }

    public virtual void UnPauseAndHide() {
        SilentUnPauseAndHide();
        onHideUI.Invoke();
    }

    public virtual void SilentUnPauseAndHide() {
        uiToShow.SetActive(false);

        GameManager.UnPauseSim();

        if (sendPlayerToSpawn)
            playerShip.GetComponent<PlayerSpawnController>().GotoSpawnPoint();
    }
}

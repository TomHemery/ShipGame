using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public string gameplaySceneName;

    private bool shownIntro = false;
    private bool shownInventoryTutorial = false;
    private bool shownStationTutorial = false;

    private GameObject playerShip;
    private PauseAndShowUIOnCollide stationUIController;

    private void Awake()
    {
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerShip.GetComponent<Inventory>().InventoryChangedEvent += OnPlayerInventoryChange;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameplaySceneName && !shownIntro) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotIntro");
            shownIntro = true;

            stationUIController = GameObject.FindGameObjectWithTag("MiningStation").GetComponent<PauseAndShowUIOnCollide>();
            stationUIController.onShowUI.AddListener(OnShowStationUI);
        }
    }

    void OnPlayerInventoryChange(object sender, EventArgs e)
    {
        if (!shownInventoryTutorial) {
            if (playerShip.GetComponent<Inventory>().Contents.ContainsKey("Stone"))
            {
                DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPickup");
                shownInventoryTutorial = true;
            }
        }
    }

    void OnShowStationUI() {
        if (shownInventoryTutorial && !shownStationTutorial)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstEnterStation");
            shownStationTutorial = true;
        }
        else if(!shownStationTutorial) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotEarlyVisitStation");
            stationUIController.UnPauseAndHide();
        }
    }
}
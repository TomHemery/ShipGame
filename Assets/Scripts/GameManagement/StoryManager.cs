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

    private GameObject playerShip;

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

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameplaySceneName && !shownIntro) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotIntro");
            shownIntro = true;
        }
    }

    void OnPlayerInventoryChange(object sender, EventArgs e)
    {
        if (!shownInventoryTutorial) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPickup");
            shownInventoryTutorial = true;
        }
    }
}
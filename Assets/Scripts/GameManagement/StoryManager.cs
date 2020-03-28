using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public string gameplaySceneName;

    private Stage stage = Stage.Intro;

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
        if (scene.name == gameplaySceneName) {
            stationUIController = GameObject.FindGameObjectWithTag("MiningStation").GetComponent<PauseAndShowUIOnCollide>();
            stationUIController.onShowUI.AddListener(OnShowStationUI);
            stationUIController.onHideUI.AddListener(OnHideStationUI);

            if (stage == Stage.Intro)
            {
                DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotIntro");
                stage = Stage.InventoryTutorial;
            }
        }
    }

    void OnPlayerInventoryChange(object sender, EventArgs e)
    {
        if (stage == Stage.InventoryTutorial)
        {
            if (playerShip.GetComponent<Inventory>().Contents.ContainsKey("Stone"))
            {
                DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPickup");
                stage = Stage.StationTutorial;
            }
        }
        else if (stage == Stage.FirstPirateEncounter) {
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 100;
            Vector2 pos = playerShip.transform.position;
            pos += offset;
            for (int i = 0; i < 2; i++) {
                Instantiate(EnemySpawner.EnemyPrefabs["BasicRebelShip"], pos + UnityEngine.Random.insideUnitCircle.normalized * 2, Quaternion.identity);
            }
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPirateEncounter");
            stage = Stage.End;
        }
    }

    void OnShowStationUI() {
        if (stage == Stage.StationTutorial)
        { //ready to show the station tutorial
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstEnterStation");
            stage = Stage.PirateTransmission;
        }
        else if(stage < Stage.StationTutorial)
        { //cheeky git tryna hide in the station
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotEarlyVisitStation");
            stationUIController.UnPauseAndHide();
        }
    }

    void OnHideStationUI() {
        if (stage == Stage.PirateTransmission) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("PirateTransmission");
            stage = Stage.FirstPirateEncounter;
        }
    }

    //stages go here in chronological (I can't spell) order
    public enum Stage {
        Intro,
        InventoryTutorial,
        StationTutorial,
        PirateTransmission,
        FirstPirateEncounter,
        End
    }
}
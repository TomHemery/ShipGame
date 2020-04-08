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
        EnemySpawner.AllEnemiesDestroyed.AddListener(OnAllEnemiesDestroyed);
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
            EnemySpawner.SpawnOnRadiusAt(pos + offset, 2, "BasicPirateShip");
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPirateEncounter");
            stage = Stage.EndFirstPirateEncounter;
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

    void OnAllEnemiesDestroyed() {
        if (stage == Stage.EndFirstPirateEncounter) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotPostFirstPirateEncounter");
            stage = Stage.End;
        }
    }

    //stages go here in chronological (I can't spell) order
    public enum Stage {
        Intro,
        InventoryTutorial,
        StationTutorial,
        PirateTransmission,
        FirstPirateEncounter,
        EndFirstPirateEncounter,
        End
    }
}
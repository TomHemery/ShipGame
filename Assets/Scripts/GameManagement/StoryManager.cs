using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public string gameplaySceneName;

    public static Stage StoryStage { get; private set; } = Stage.Intro;

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

            if (StoryStage == Stage.Intro)
            {
                DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotIntro");
                StoryStage = Stage.InventoryTutorial;
            }
        }
    }

    void OnPlayerInventoryChange(object sender, EventArgs e)
    {
        if (StoryStage == Stage.InventoryTutorial)
        {
            if (playerShip.GetComponent<Inventory>().Contents.ContainsKey("Stone"))
            {
                DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPickup");
                StoryStage = Stage.StationTutorial;
            }
        }
        else if (StoryStage == Stage.FirstPirateEncounter) {
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 100;
            Vector2 pos = playerShip.transform.position;
            EnemySpawner.SpawnOnRadiusAt(pos + offset, 2, "BasicPirateShip");
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPirateEncounter");
            StoryStage = Stage.EndFirstPirateEncounter;
        }
    }

    void OnShowStationUI() {
        if (StoryStage == Stage.StationTutorial)
        { //ready to show the station tutorial
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstEnterStation");
            StoryStage = Stage.PirateTransmission;
        }
        else if(StoryStage < Stage.StationTutorial)
        { //cheeky git tryna hide in the station
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotEarlyVisitStation");
            stationUIController.UnPauseAndHide();
        }
    }

    void OnHideStationUI() {
        if (StoryStage == Stage.PirateTransmission) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("PirateTransmission");
            StoryStage = Stage.FirstPirateEncounter;
        }

        //saves the game 
        Save.SaveGame();
    }

    void OnAllEnemiesDestroyed() {
        if (StoryStage == Stage.EndFirstPirateEncounter) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotPostFirstPirateEncounter");
            StoryStage = Stage.End;
        }
    }

    //stages go here in chronological (I can't spell) order
    [Serializable]
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
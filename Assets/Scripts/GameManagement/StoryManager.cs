using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public static Stage StoryStage { get; private set; } = Stage.Intro;

    private GameObject playerShip;
    private GameObject miningStation;
    private PauseAndShowUIOnCollide stationUIController;

    public GameObject jumpPanelCover;

    public static StoryManager Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        miningStation = GameObject.FindGameObjectWithTag("MiningStation");

        stationUIController = miningStation.GetComponent<PauseAndShowUIOnCollide>();
        GameManager.OnAreaLoaded.AddListener(OnAreaLoaded);
    }

    private void OnEnable()
    {
        EnemySpawner.AllEnemiesDestroyed.AddListener(OnAllEnemiesDestroyed);
        AsteroidDestroyedEvent.OnDestroyEvent.AddListener(OnAsteroidDestroyed);
        stationUIController.onShowUI.AddListener(OnShowStationUI);
        stationUIController.onHideUI.AddListener(OnHideStationUI);
        GalaxyMap.Instance.OnShowMap.AddListener(OnShowGalaxyMap);
    }

    void OnAreaLoaded()
    {
        if (GameManager.CurrentArea.systemName == GameManager.Instance.firstArea) {
            if (StoryStage == Stage.Intro)
            {
                DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotIntro");
                StoryStage = Stage.InventoryTutorial;
            }
            playerShip.GetComponent<PlayerSpawnController>().GotoSpawnPoint();
        }
    }

    void OnAsteroidDestroyed()
    {
        if (StoryStage == Stage.InventoryTutorial)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPickup");
            StoryStage = Stage.StationTutorial;
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
        if (StoryStage == Stage.JumpTutorial) {
            jumpPanelCover.SetActive(false);
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotJumpTutorial");
            StoryStage = Stage.GalaxyMapTutorial;
        }
        else if (StoryStage == Stage.StationTutorial)
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

        //saves the game if we have completed the station tutorial
        if(StoryStage >= Stage.StationTutorial)
            Save.SaveGame();
    }

    void OnShowGalaxyMap() {
        if (StoryStage == Stage.GalaxyMapTutorial) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotGalaxyMapTutorial");
            StoryStage = Stage.End;
        }
    }

    void OnAllEnemiesDestroyed() {
        if (StoryStage == Stage.EndFirstPirateEncounter) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotPostFirstPirateEncounter");
            StoryStage = Stage.JumpTutorial;
        }
    }

    public void SetStage(Stage s) {
        StoryStage = s;
        if (s >= Stage.JumpTutorial)
            jumpPanelCover.SetActive(false);
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
        JumpTutorial,
        GalaxyMapTutorial,
        End
    }
}
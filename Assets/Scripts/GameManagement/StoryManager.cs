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
    private MiningStationUIToggle stationUIController;

    public GameObject jumpPanelCover;
    public GameObject jumpRefueler;

    public GameObject toggleCraftingPanelButton;

    public GameObject saveGameText;

    public static StoryManager Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        miningStation = GameObject.FindGameObjectWithTag("MiningStation");

        stationUIController = miningStation.GetComponent<MiningStationUIToggle>();
        GameManager.OnAreaLoaded.AddListener(OnAreaLoaded);
        DialoguePanel.MainDialoguePanel.OnDialoguePanelClosed.AddListener(OnDialogueClosed);
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
                SetStage(Stage.InventoryTutorial);
            }
            playerShip.GetComponent<PlayerSpawnController>().GotoSpawnPoint();
        }
    }

    void OnAsteroidDestroyed()
    {
        if (StoryStage == Stage.InventoryTutorial)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPickup");
            SetStage(Stage.StationTutorial);
        }
        else if (StoryStage == Stage.FirstPirateEncounter) {
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 100;
            Vector2 pos = playerShip.transform.position;
            EnemySpawner.SpawnOnRadiusAt(pos + offset, 2, "BasicPirateShip");
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstPirateEncounter");
            SetStage(Stage.EndFirstPirateEncounter);
        }
    }

    void OnShowStationUI() {
        if (StoryStage == Stage.JumpTutorial) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotJumpTutorial");
            SetStage(Stage.CraftingTutorial);
        }
        else if (StoryStage == Stage.StationTutorial)
        { //ready to show the station tutorial
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotOnFirstEnterStation");
            SetStage(Stage.PirateTransmission);
        }
        else if(StoryStage < Stage.StationTutorial)
        { //cheeky git tryna hide in the station
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotEarlyVisitStation");
            stationUIController.UnPauseAndHide();
        }
    }

    void OnHideStationUI() {
        if (StoryStage == Stage.PirateTransmission)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("PirateTransmission");
            SetStage(Stage.FirstPirateEncounter);
        }

        else if (StoryStage == Stage.FirstRebelContact) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FirstRebelContact");
            SetStage(Stage.SecondRebelContact);
        }

        //saves the game if we have completed the station tutorial
        if (StoryStage >= Stage.StationTutorial)
        {
            Save.SaveGame();
            saveGameText.SetActive(true);
        }
    }

    public void OnToggleCraftingPanel()
    {
        if(StoryStage == Stage.CraftingTutorial)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotCraftingTutorial");
            SetStage(Stage.GalaxyMapTutorial);
        }
    }

    public void OnDialogueClosed() {
        if (StoryStage == Stage.SecondPirateEncounter) {
            StartCoroutine("SecondPirateEncounter");
        }
    }

    private IEnumerable SecondPirateEncounter()
    {
        yield return new WaitForSeconds(5.0f);
        DialoguePanel.MainDialoguePanel.OpenDialogue("SecondPirateEncounter");
        SetStage(Stage.SecondRebelContact);
    }

    void OnShowGalaxyMap() {
        if (StoryStage == Stage.GalaxyMapTutorial) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotGalaxyMapTutorial");
            SetStage(Stage.FirstRebelContact);
        }
    }

    void OnAllEnemiesDestroyed() {
        if (StoryStage == Stage.EndFirstPirateEncounter) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotPostFirstPirateEncounter");
            SetStage(Stage.JumpTutorial);
        }
    }

    public void SetStage(Stage s) {
        StoryStage = s;

        jumpPanelCover.SetActive(s < Stage.JumpTutorial);

        jumpRefueler.SetActive(s >= Stage.JumpTutorial);
        toggleCraftingPanelButton.SetActive(s >= Stage.JumpTutorial);
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
        CraftingTutorial,
        GalaxyMapTutorial,
        FirstRebelContact,
        SecondPirateEncounter,
        SecondRebelContact,
        End
    }
}
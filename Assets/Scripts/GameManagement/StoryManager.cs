﻿using System;
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
    public TabMenu miningStationUITabMenu;
    private TabMenuButton craftingTabButton;
    public int craftingTabButtonIndex = 1;

    public GameObject jumpPanelCover;
    public GameObject jumpRefueler;


    public GameObject saveGameText;

    public static StoryManager Instance { get; private set; } = null;

    public DestinationButton deadSectorDestButton;

    //##MONO BEHAVIOUR EVENTS##
    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        miningStation = GameObject.FindGameObjectWithTag("MiningStation");

        stationUIController = miningStation.GetComponent<MiningStationUIToggle>();
        GameManager.OnAreaLoaded.AddListener(OnAreaLoaded);
        DialoguePanel.MainDialoguePanel.OnDialoguePanelClosed.AddListener(OnDialogueClosed);
    }

    //registers events that we need to hook in to
    private void Start()
    {
        EnemySpawner.AllEnemiesDestroyed.AddListener(OnAllEnemiesDestroyed);
        AsteroidDestroyedEvent.OnDestroyEvent.AddListener(OnAsteroidDestroyed);
        stationUIController.onShowUI.AddListener(OnShowStationUI);
        stationUIController.onHideUI.AddListener(OnHideStationUI);
        GalaxyMap.Instance.OnShowMap.AddListener(OnShowGalaxyMap);
        GameManager.OnPlayerDeath.AddListener(OnPlayerDied);
        MiningStationController.Instance.OnJumpCompleted.AddListener(OnJumpCompleted);
        craftingTabButton = miningStationUITabMenu.TabMenuButtons[craftingTabButtonIndex];
    }
    //##END MONOBEHAVIOUR EVENTS##

    //##EVENT LISTENERS##
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

        if (StoryStage == Stage.EmpireStrikerEncounter)
        {
            StartCoroutine(AwaitStrikers());
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
            EnemySpawner.SpawnAt(pos + offset, "BasicPirateShip");
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

        //saves the game if we have completed the station tutorial
        if (StoryStage >= Stage.StationTutorial)
        {
            Save.SaveGame();
            saveGameText.SetActive(true);
        }
    }

    void OnJumpCompleted()
    {
        if (StoryStage == Stage.FirstRebelContact)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FirstRebelContact");
            SetStage(Stage.SecondPirateEncounter);
        }
        else if (GameManager.CurrentArea.systemName == "Area_2_DeadSector" && StoryStage == Stage.ArrivalAtDeadZone) 
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("DeadSectorArrival");
            SetStage(Stage.End);
        }

        Save.SaveGame();
        saveGameText.SetActive(true);
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
        if (StoryStage == Stage.EmpireStrikerEncounter)
        {
            Save.SaveGame();
            saveGameText.SetActive(true);
            StartCoroutine(AwaitStrikers());
        }
        else if (StoryStage == Stage.PostStrikers) {
            playerShip.GetComponent<HealthAndShieldsResource>().FillResource();
        }
    }

    void OnShowGalaxyMap() {
        if (StoryStage == Stage.GalaxyMapTutorial) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotGalaxyMapTutorial");
            SetStage(Stage.FirstRebelContact);
        }
    }

    void OnAllEnemiesDestroyed() {
        if (StoryStage == Stage.EndFirstPirateEncounter)
        {
            DialoguePanel.MainDialoguePanel.OpenDialogue("FriendBotPostFirstPirateEncounter");
            SetStage(Stage.JumpTutorial);
        }
        else if (StoryStage == Stage.SecondRebelContact) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("SecondRebelContact");
            SetStage(Stage.EmpireStrikerEncounter);
        }
        else if (StoryStage == Stage.PostStrikers) {
            DialoguePanel.MainDialoguePanel.OpenDialogue("RebelCoordinateDialogue");
            SetStage(Stage.ArrivalAtDeadZone);
        }
    }

    void OnPlayerDied()
    {
        //stop all running coroutines to prevent any awkward stuff happening post player death
        StopAllCoroutines();
    }
    //##END EVENT LISTENERS##

    //###COROUTINE ENUMERATORS###
    private IEnumerator SecondPirateEncounter()
    {
        float t = 0.0f;

        while (t < 5.0f)
        {
            yield return null;
            if (!GameManager.SimPaused)
                t += Time.deltaTime;
        }

        DialoguePanel.MainDialoguePanel.OpenDialogue("SecondPirateEncounter");
        Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 100;
        Vector2 pos = playerShip.transform.position;
        EnemySpawner.SpawnAt(pos + offset, "BasicPirateShip", 1);
        EnemySpawner.SpawnAt(pos + offset + UnityEngine.Random.insideUnitCircle.normalized * 5.0f, "RocketLauncherPirateShip", 1);
        SetStage(Stage.SecondRebelContact);
    }

    private IEnumerator AwaitStrikers() {
        playerShip.GetComponent<PlayerShipController>().StoryControlOverride = true;
        MusicPlayer.Instance.FadeToNewTrack(1.0f, "HeldDissonance");
        yield return new WaitForSeconds(5.0f);
        DialoguePanel.MainDialoguePanel.OpenDialogue("StrikerDeployment");
        Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 100;
        Vector2 pos = playerShip.transform.position;
        EnemySpawner.SpawnAt(pos + offset, "EmpireStriker", 1);
        playerShip.GetComponent<PlayerShipController>().StoryControlOverride = false;
        
        SetStage(Stage.PostStrikers);
    }
    //###END COROUTINE ENUMERATORS###
    
    //##SETTERS##
    public void SetStage(Stage s) {
        StoryStage = s;

        jumpPanelCover.SetActive(s < Stage.JumpTutorial);

        jumpRefueler.SetActive(s >= Stage.JumpTutorial);
        if (craftingTabButton != null) craftingTabButton.gameObject.SetActive(s >= Stage.JumpTutorial);


        if (s == Stage.SecondPirateEncounter)
        {
            StartCoroutine(SecondPirateEncounter());
        }

        if (StoryStage >= Stage.PostStrikers)
        {
            deadSectorDestButton.Unlock();
        }
    }
    //##END SETTERS##

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
        EmpireStrikerEncounter,
        PostStrikers,
        ArrivalAtDeadZone,
        End
    }
}
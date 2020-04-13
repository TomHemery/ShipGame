using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    public string firstArea;
    public static Area CurrentArea { get; private set; }

    public GameObject DeathScreen;
    public GameObject MainMenu;

    private GameObject playerShip;
    private GameObject miningStation;

    bool playerDied = false;

    public static bool SimPaused { get; private set; } = false;
    public static UnityEvent OnSimPause { get; private set; } = new UnityEvent();
    public static UnityEvent OnSimUnPause { get; private set; } = new UnityEvent();

    public static UnityEvent OnAreaLoaded { get; private set; } = new UnityEvent();
    public static UnityEvent OnPlayerDeath { get; private set; } = new UnityEvent();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        miningStation = GameObject.FindGameObjectWithTag("MiningStation");
    }

    private void Start()
    {
        DeathScreen.SetActive(false);
        ShowMainMenu();
    }

    public static void LoadArea(Area area)
    {
        CurrentArea = area;
        SceneManager.LoadScene(area.systemName);
        OnAreaLoaded.Invoke();
    }

    public void KillPlayerBy(PlayerDeathTypes deathType) {
        if (!playerDied)
        {
            Text deathText = DeathScreen.GetComponentInChildren<Text>();
            deathText.text = deathType.ToString();
            playerDied = true;
            StartCoroutine(PlayerDeath());
        }
    }

    public void ShowMainMenu() {
        DeathScreen.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void LoadGameFromSave(Save save) {
        playerDied = false;

        playerShip.transform.position = playerShip.GetComponent<PlayerSpawnController>().spawnPoint.position;

        playerShip.GetComponent<HullSpawner>().hull = save.playerHull;
        playerShip.GetComponent<HullSpawner>().weapons = save.playerWeapons;
        playerShip.GetComponent<HullSpawner>().SpawnHull();

        playerShip.GetComponent<HealthAndShieldsResource>().SetHealth(save.playerHealth);
        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxHealth(save.playerMaxHealth);
        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxShieldValue(save.playerMaxShields);
        playerShip.GetComponent<HealthAndShieldsResource>().exploded = false;
        playerShip.GetComponent<OxygenResource>().SetResource(save.playerO2Levels);

        playerShip.GetComponent<Inventory>().SetContents(save.playerInventoryContents);
        miningStation.GetComponent<Inventory>().SetContents(save.miningStationInventoryContents);
        miningStation.GetComponent<JumpResource>().SetResource(save.jumpDriveFillLevel);

        if (save.hullRepairerContents > 0) {
            InventoryItem ironItem = PrefabDatabase.PrefabDictionary["Iron"].GetComponent<PickUpOnContact>().m_inventoryItem;
            ironItem.quantity = save.hullRepairerContents;
            miningStation.GetComponent<MiningStationController>().m_HullRepairer.slot.TryCreateFrameFor(
                ironItem     
            );
        }

        if (save.jumpDriveContents > 0)
        {
            InventoryItem crystalItem = PrefabDatabase.PrefabDictionary["Crystal"].GetComponent<PickUpOnContact>().m_inventoryItem;
            crystalItem.quantity = save.jumpDriveContents;
            miningStation.GetComponent<MiningStationController>().m_JumpDriveFueler.slot.TryCreateFrameFor(
                crystalItem
            );
        }

        if (save.o2GenContents > 0)
        {
            InventoryItem iceItem = PrefabDatabase.PrefabDictionary["Ice"].GetComponent<PickUpOnContact>().m_inventoryItem;
            iceItem.quantity = save.o2GenContents;
            miningStation.GetComponent<MiningStationController>().m_O2Gen.slot.TryCreateFrameFor(
                iceItem
            );
        }

        StoryManager.Instance.SetStage(save.storyStage);

        LoadArea(AreaDatabase.AreaDictionary[firstArea]);
    }

    public void StartNewGame()
    {
        playerDied = false;
        LoadArea(AreaDatabase.AreaDictionary[firstArea]);

        playerShip.GetComponent<HullSpawner>().SpawnDefaultHull();
        playerShip.GetComponent<HealthAndShieldsResource>().exploded = false;

        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxHealth(HealthResource.DEFAULT_MAX_HEALTH);
        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxShieldValue(HealthAndShieldsResource.DEFAULT_MAX_SHIELDS);

        playerShip.GetComponent<HealthAndShieldsResource>().FillResource();
    }

    IEnumerator PlayerDeath()
    { 
        yield return new WaitForEndOfFrame();

        DeathScreen.GetComponent<DeathScreen>().CaptureScreen();
        DeathScreen.SetActive(true);

        playerShip.GetComponent<HullSpawner>().DestroyHull();
        MusicPlayer.Instance.FadeOut(2.0f);

        OnPlayerDeath.Invoke();
    }

    public static void PauseSim() {
        SimPaused = true;
        OnSimPause.Invoke();
    }

    public static void UnPauseSim() {
        SimPaused = false;
        OnSimUnPause.Invoke();
    }
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
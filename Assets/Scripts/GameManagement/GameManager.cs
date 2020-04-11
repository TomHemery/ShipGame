using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    public string FirstScene;
    public GameObject DeathScreen;
    public GameObject MainMenu;

    private GameObject playerShip;

    bool playerDied = false;

    public static bool SimPaused { get; private set; } = false;
    public static UnityEvent onSimPause = null;
    public static UnityEvent onSimUnPause = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (onSimPause == null) {
            onSimPause = new UnityEvent();
            onSimUnPause = new UnityEvent();
        }
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }

    private void Start()
    {
        DeathScreen.SetActive(false);
        ShowMainMenu();
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnPlayerDeath(PlayerDeathTypes deathType) {
        if (!playerDied)
        {
            Text deathText = DeathScreen.GetComponentInChildren<Text>();
            deathText.text = deathType.ToString();
            playerDied = true;
            StartCoroutine(ShowDeathScreen());
        }
    }

    public void ShowMainMenu() {
        DeathScreen.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void LoadGameFromSave(Save save) {
        playerDied = false;
        GameObject miningStation = GameObject.FindGameObjectWithTag("MiningStation");

        playerShip.transform.position = playerShip.GetComponent<PlayerSpawnController>().spawnPoint.position;

        playerShip.GetComponent<HullSpawner>().hull = save.playerHull;
        playerShip.GetComponent<HullSpawner>().weapons = save.playerWeapons;
        playerShip.GetComponent<HullSpawner>().SpawnHull();

        playerShip.GetComponent<HealthAndShieldsResource>().SetHealth(save.playerHealth);
        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxHealth(save.playerMaxHealth);
        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxShieldValue(save.playerMaxShields);
        playerShip.GetComponent<HealthAndShieldsResource>().exploded = false;

        playerShip.GetComponent<Inventory>().SetContents(save.playerInventoryContents);
        miningStation.GetComponent<Inventory>().SetContents(save.miningStationInventoryContents);

        StoryManager.StoryStage = save.storyStage;

        LoadScene(FirstScene);
        MainMenu.gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        playerDied = false;
        LoadScene(FirstScene);
        MainMenu.gameObject.SetActive(false);

        playerShip.GetComponent<HullSpawner>().SpawnDefaultHull();
        playerShip.GetComponent<HealthAndShieldsResource>().exploded = false;

        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxHealth(HealthResource.DEFAULT_MAX_HEALTH);
        playerShip.GetComponent<HealthAndShieldsResource>().SetMaxShieldValue(HealthAndShieldsResource.DEFAULT_MAX_SHIELDS);

        playerShip.GetComponent<HealthAndShieldsResource>().FillResource();
    }

    IEnumerator ShowDeathScreen()
    { 
        yield return new WaitForEndOfFrame();

        DeathScreen.GetComponent<DeathScreen>().CaptureScreen();
        DeathScreen.SetActive(true);

        playerShip.GetComponent<HullSpawner>().DestroyHull();
        MusicPlayer.Instance.FadeOut(2.0f);
    }

    public static void PauseSim() {
        SimPaused = true;
        onSimPause.Invoke();
    }

    public static void UnPauseSim() {
        SimPaused = false;
        onSimUnPause.Invoke();
    }
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
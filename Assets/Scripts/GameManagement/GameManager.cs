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
    }

    private void Start()
    {
        DeathScreen.SetActive(false);
        if (Instance == this)
            MainMenu.SetActive(true);
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

    public void LoadGameFromSave(Save save) {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerShip");
        GameObject miningStation = GameObject.FindGameObjectWithTag("MiningStation");

        player.GetComponent<HullSpawner>().hull = save.playerHull;
        player.GetComponent<HullSpawner>().weapons = save.playerWeapons;
        player.GetComponent<HullSpawner>().SpawnHull();

        player.GetComponent<HealthAndShieldsResourceManager>().SetHealth(save.playerHealth);
        player.GetComponent<HealthAndShieldsResourceManager>().SetMaxHealth(save.playerMaxHealth);
        player.GetComponent<HealthAndShieldsResourceManager>().SetMaxShields(save.playerMaxShields);

        player.GetComponent<Inventory>().SetContents(save.playerInventoryContents);
        miningStation.GetComponent<Inventory>().SetContents(save.miningStationInventoryContents);

        StoryManager.StoryStage = save.storyStage;

        LoadScene(FirstScene);
        MainMenu.gameObject.SetActive(false);
    }

    IEnumerator ShowDeathScreen()
    {
        yield return new WaitForEndOfFrame();

        DeathScreen.GetComponent<DeathScreen>().CaptureScreen();
        DeathScreen.SetActive(true);
    }

    public static void PauseSim() {
        SimPaused = true;
        onSimPause.Invoke();
    }

    public static void UnPauseSim() {
        SimPaused = false;
        onSimUnPause.Invoke();
    }

    public void StartNewGame() {
        LoadScene(FirstScene);
        MainMenu.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<HullSpawner>().SpawnHull();
    }
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
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
        Debug.Log("Loading game from save");
        Debug.Log("Story Stage: " + save.storyStage);
        Debug.Log("Player Inventory: " + save.playerInventoryContents);
        Debug.Log("MiningStaiton Inventory: " + save.miningStationInventoryContents);
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
    }
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
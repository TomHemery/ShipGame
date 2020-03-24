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
            LoadScene(FirstScene);
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
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
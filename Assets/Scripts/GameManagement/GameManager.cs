using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    public string FirstScene;
    public GameObject DeathScreen;
    bool playerDied = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
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
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
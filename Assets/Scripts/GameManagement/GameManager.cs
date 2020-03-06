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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        DeathScreen.SetActive(false);
    }

    private void Start()
    {
        if (Instance == this)
            LoadScene(FirstScene);
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnPlayerDeath(PlayerDeathTypes deathType) {
        DeathScreen.SetActive(true);

        Text deathText = DeathScreen.GetComponentInChildren<Text>();
        deathText.text = deathType.ToString();
    }
}

public enum PlayerDeathTypes { 
    OutOfOxygen,
    Exploded
}
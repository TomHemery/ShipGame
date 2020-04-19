using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public static MainMenuController Instance { get; private set; } = null;

    public Dropdown loadGameDropdown;
    public Button loadGameButton;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.PauseSim();

        loadGameDropdown.ClearOptions();
        string[] saveNames = Save.GetAllSaveNames();

        if (saveNames.Length > 0)   
        {
            loadGameButton.interactable = true;
            loadGameDropdown.gameObject.SetActive(true);
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            foreach (string saveName in saveNames)
            {
                options.Add(new Dropdown.OptionData(saveName));
            }
            loadGameDropdown.AddOptions(options);
        }
        else {
            loadGameButton.interactable = false;
            loadGameDropdown.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        GameManager.UnPauseSim();
    }

    public void OnLoadGameButtonPressed() {
        gameObject.SetActive(false);
        GameManager.Instance.LoadGameFromSave(
            Save.LoadGame(loadGameDropdown.options[loadGameDropdown.value].text)
        );
    }

    public void OnNewGameButtonPressed() {
        gameObject.SetActive(false);
        GameManager.Instance.StartNewGame();
    }
}

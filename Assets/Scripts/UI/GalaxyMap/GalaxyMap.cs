using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyMap : MonoBehaviour
{

    public static GalaxyMap Instance { get; private set; } = null;

    private List<DestinationButton> destinationButtons = new List<DestinationButton>();
    private JumpResource stationJumpResource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameManager.OnAreaLoaded.AddListener(OnAreaChanged);

            foreach (Transform child in transform)
            {
                if (child.GetComponent<DestinationButton>() != null)
                {
                    destinationButtons.Add(child.GetComponent<DestinationButton>());
                }
            }
            stationJumpResource = GameObject.FindGameObjectWithTag("MiningStation").GetComponent<JumpResource>();
        }
    }

    private void Start()
    {
        HideMap();
    }

    public void OnAreaChanged() {
        foreach (DestinationButton destination in destinationButtons)
            destination.SetAsInvalidTarget();

        foreach(DestinationButton destination in destinationButtons){
            if (destination.areaName == GameManager.CurrentArea.systemName) {
                destination.SetAsCurrent();
                foreach (DestinationButton neighbour in destination.neighbours)
                {
                    neighbour.SetAsValidTarget();
                }
                break;
            }
        }
    }

    public void ShowMap() {
        gameObject.SetActive(true);
    }

    public void HideMap() {
        gameObject.SetActive(false);
    }

    public void DestinationSelected(string areaName) {
        HideMap();
        stationJumpResource.EmptyResource();
        GameManager.LoadArea(AreaDatabase.AreaDictionary[areaName]);
    }
}

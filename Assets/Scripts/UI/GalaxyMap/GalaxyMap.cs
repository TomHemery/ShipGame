using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GalaxyMap : MonoBehaviour
{

    public static GalaxyMap Instance { get; private set; } = null;

    private List<DestinationButton> destinationButtons = new List<DestinationButton>();
    private JumpResource stationJumpResource;

    public GameObject lineObject;

    public UnityEvent OnShowMap { get; private set; } = new UnityEvent();
    public UnityEvent OnHideMap { get; private set; } = new UnityEvent();

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
                    foreach (DestinationButton neighbour in child.GetComponent<DestinationButton>().neighbours) {
                        GameObject line = Instantiate(lineObject, transform);
                        line.GetComponent<Line>().Set(child.transform.position, neighbour.transform.position, 1.0f, GetComponent<Canvas>());
                    }
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
        OnShowMap.Invoke();
    }

    public void HideMap() {
        gameObject.SetActive(false);
        OnHideMap.Invoke();
    }

    public void DestinationSelected(string areaName) {
        HideMap();
        stationJumpResource.EmptyResource();
        GameManager.LoadArea(AreaDatabase.AreaDictionary[areaName]);
    }
}

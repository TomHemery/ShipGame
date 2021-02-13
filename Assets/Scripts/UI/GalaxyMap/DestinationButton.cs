using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DestinationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Text nameText;

    public Sprite validDestSprite;
    public Sprite currentDestSprite;

    public Image overlayImage;
    

    public string areaName;
    public DestinationButton[] neighbours;

    [SerializeField]
    private bool unlocked = false;


    private void Awake()
    {
        button = GetComponent<Button>();
        nameText = GetComponentInChildren<Text>();
        nameText.gameObject.SetActive(false);
    }

    private void Start()
    {
        nameText.text = AreaDatabase.AreaDictionary[areaName].prettyName;
        gameObject.SetActive(unlocked);
    }

    public bool IsUnlocked() {
        return unlocked;
    }

    public void Unlock() {
        unlocked = true;
        gameObject.SetActive(true);
        GalaxyMap.Instance.AddDestination(this);
    }

    public void Lock() {
        unlocked = false;
        gameObject.SetActive(false);
    }

    public void SetAsCurrent() {
        overlayImage.color = Color.white;
        overlayImage.sprite = currentDestSprite;

        button.interactable = false;
    }

    public void SetAsValidTarget() {
        overlayImage.color = Color.white;
        overlayImage.sprite = validDestSprite;
        button.interactable = true;
    }

    public void SetAsInvalidTarget() {
        overlayImage.color = Color.clear;
        overlayImage.sprite = null;
        button.interactable = false;
    }

    public void OnPressed()
    {
        GalaxyMap.Instance.DestinationSelected(areaName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        nameText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameText.gameObject.SetActive(false);
    }
}

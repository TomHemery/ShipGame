using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DestinationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button m_Button;
    private Text m_nameText;

    public Sprite validDestSprite;
    public Sprite currentDestSprite;

    public Image m_overlayImage;
    

    public string areaName;
    public DestinationButton[] neighbours;

    [SerializeField]
    private bool unlocked = false;


    private void Awake()
    {
        m_Button = GetComponent<Button>();
        m_nameText = GetComponentInChildren<Text>();
        m_nameText.gameObject.SetActive(false);
    }

    private void Start()
    {
        m_nameText.text = AreaDatabase.AreaDictionary[areaName].prettyName;
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
        m_overlayImage.color = Color.white;
        m_overlayImage.sprite = currentDestSprite;

        m_Button.interactable = false;
    }

    public void SetAsValidTarget() {
        m_overlayImage.color = Color.white;
        m_overlayImage.sprite = validDestSprite;
        m_Button.interactable = true;
    }

    public void SetAsInvalidTarget() {
        m_overlayImage.color = Color.clear;
        m_overlayImage.sprite = null;
        m_Button.interactable = false;
    }

    public void OnPressed()
    {
        GalaxyMap.Instance.DestinationSelected(areaName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_nameText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_nameText.gameObject.SetActive(false);
    }
}

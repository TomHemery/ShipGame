using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestinationButton : MonoBehaviour
{
    private Button m_Button;

    public Sprite validDestSprite;
    public Sprite currentDestSprite;

    public Image m_overlayImage;

    public string areaName;
    public DestinationButton[] neighbours;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
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
}

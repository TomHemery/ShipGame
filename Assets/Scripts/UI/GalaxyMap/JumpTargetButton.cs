using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpTargetButton : MonoBehaviour
{
    private Button m_Button;

    public Sprite validTargetTexture;
    public Sprite currentTargetTexture;

    public Image m_overlayImage;

    public string associatedAreaName;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetAsInvalidTarget();
    }

    public void SetAsCurrent() {
        m_overlayImage.color = Color.white;
        m_overlayImage.sprite = currentTargetTexture;
        m_Button.interactable = false;
    }

    public void SetAsValidTarget() {
        m_overlayImage.color = Color.white;
        m_overlayImage.sprite = validTargetTexture;
        m_Button.interactable = true;
    }

    public void SetAsInvalidTarget() {
        m_overlayImage.color = Color.clear;
        m_overlayImage.sprite = null;
        m_Button.interactable = false;
    }

}

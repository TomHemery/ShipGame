using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFrameBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    public GameObject nameText;

    public Image itemFrameImage;
    public Color baseColour;
    public Color onHighlightColour;

    // Start is called before the first frame update
    void Start()
    {
        if (nameText != null) nameText.SetActive(false);
        itemFrameImage.color = baseColour;
    }

    public void OnPointerEnter(PointerEventData eventdata)
    {
        nameText.SetActive(true);
        itemFrameImage.color = onHighlightColour;
    }

    public void OnPointerExit(PointerEventData eventdata)
    {
        nameText.SetActive(false);
        itemFrameImage.color = baseColour;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        eventData.Use();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        eventData.Use();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
    }

}

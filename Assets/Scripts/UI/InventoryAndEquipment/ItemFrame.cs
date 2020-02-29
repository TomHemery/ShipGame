using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public InventoryItem m_inventoryItem;

    public GameObject nameText;
    public GameObject quantityText;

    public Image itemFrameImage;
    public Color baseColour;
    public Color onHighlightColour;
    private Image backgroundImage;

    private RectTransform mTransform;
    private Vector2 dragStartPos;

    private void Awake()
    {
        mTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        backgroundImage.color = baseColour;
    }

    void Start()
    {
        if (nameText != null) nameText.SetActive(false);

        nameText.GetComponent<Text>().text = m_inventoryItem.prettyName;
        quantityText.GetComponent<Text>().text = m_inventoryItem.quantity.ToString();
        itemFrameImage.sprite = m_inventoryItem.inventorySprite;
    }

    public void OnPointerEnter(PointerEventData eventdata)
    {
        nameText.SetActive(true);
        backgroundImage.color = onHighlightColour;
    }

    public void OnPointerExit(PointerEventData eventdata)
    {
        nameText.SetActive(false);
        backgroundImage.color = baseColour;
    }

    public void OnDrag(PointerEventData eventData)
    { 
        mTransform.position = Input.mousePosition;
        eventData.Use();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = mTransform.position;
        mTransform.pivot = new Vector2(0.5f, 0.5f);
        mTransform.SetParent(mTransform.root);
        eventData.Use();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mTransform.pivot = new Vector2(0, 1);

        GraphicRaycaster mRaycaster = transform.root.GetComponent<GraphicRaycaster>();
        PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        mRaycaster.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results)
        {
            //TODO look for slot
        }
        
        //TODO reset
        eventData.Use();
    }
}

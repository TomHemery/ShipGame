using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public InventoryItem m_inventoryItem;
    public ItemFrameLayoutController parentController;

    public GameObject nameText;
    public GameObject quantityText;

    public Image itemFrameImage;
    public Color baseColour;
    public Color onHighlightColour;

    private RectTransform mTransform;
    private Transform originalParentTransform;
    private Vector2 dragStartPos;

    private void Awake()
    {
        mTransform = GetComponent<RectTransform>();
    }

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

    public void OnDrag(PointerEventData eventData)
    { 
        mTransform.position = Input.mousePosition;
        eventData.Use();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = mTransform.position;
        mTransform.pivot = new Vector2(0.5f, 0.5f);
        originalParentTransform = mTransform.parent;
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
            if (result.gameObject.GetComponent<ItemFrameLayoutController>() != null)
            {
                if (result.gameObject.GetComponent<ItemFrameLayoutController>().TryAddItemFrameFor(m_inventoryItem)) {
                    parentController.TryRemoveItemFrameFor(m_inventoryItem);
                    eventData.Use();
                    Destroy(gameObject);
                    return;
                }
            }
        }
        
        mTransform.position = dragStartPos;
        mTransform.SetParent(originalParentTransform);
        eventData.Use();
    }
}

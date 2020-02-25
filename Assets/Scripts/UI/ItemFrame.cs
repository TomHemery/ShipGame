using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    /// System item name
    /// </summary>
    public string itemName;
    /// <summary>
    /// System item quantity
    /// </summary>
    public int itemQuantity;

    public GameObject nameText;
    public GameObject quantityText;

    [HideInInspector]
    public InventoryUIController parentInventoryController;

    public Image itemFrameImage;
    public Color baseColour;
    public Color onHighlightColour;

    private RectTransform mTransform;

    private Vector2 dragStartPos;

    private void Awake()
    {
        mTransform = GetComponent<RectTransform>();
    }

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
            GameObject potentialUI = result.gameObject.transform.parent.gameObject;
            if (result.gameObject.CompareTag("InventoryBackground") && 
                (parentInventoryController == null || potentialUI != parentInventoryController.gameObject))
            {
                InventoryUIController targetController = potentialUI.GetComponent<InventoryUIController>();
                Inventory targetInventory = targetController.targetInventory;
                if (targetInventory.TryAddItem(itemName, itemQuantity)) {
                    if (parentInventoryController != null)
                    {
                        parentInventoryController.targetInventory.TryRemoveItem(itemName, itemQuantity);
                        parentInventoryController.UpdateContents();
                    }
                    targetController.UpdateContents();
                    eventData.Use();
                    Destroy(gameObject);
                    return;
                }
            }
        }


        mTransform.position = dragStartPos;
        mTransform.SetParent(parentInventoryController.transform);
        eventData.Use();
    }
}

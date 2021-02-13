using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public InventoryItem inventoryItem;

    public GameObject nameText;
    public GameObject quantityText;

    public Image itemFrameImage;
    public Color baseColour;
    public Color onHighlightColour;
    private Image backgroundImage;

    private RectTransform mTransform;
    private Vector2 dragStartPos;
    public Slot parentSlot;

    private bool dragging = false;

    private void Awake()
    {
        mTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        backgroundImage.color = baseColour;

        mTransform.localPosition = Vector2.zero;
        mTransform.anchoredPosition = Vector2.zero;
        //mTransform.localScale = Vector2.zero;
        mTransform.sizeDelta = Vector2.zero;
        mTransform.ForceUpdateRectTransforms();
    }

    void Start()
    {
        if (nameText != null) nameText.SetActive(false);

        nameText.GetComponent<Text>().text = inventoryItem.prettyName;
        quantityText.GetComponent<Text>().text = inventoryItem.quantity.ToString();
        itemFrameImage.sprite = inventoryItem.inventorySprite;
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
        parentSlot.RemoveItemFrame();
        dragStartPos = mTransform.position;
        mTransform.pivot = new Vector2(0.5f, 0.5f);
        mTransform.SetParent(mTransform.root);
        eventData.Use();
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mTransform.pivot = new Vector2(0, 1);

        GraphicRaycaster mRaycaster = transform.root.GetComponent<GraphicRaycaster>();
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        mRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            //TODO look for slot
            if (result.gameObject.GetComponent<Slot>() != null) {
                Slot s = result.gameObject.GetComponent<Slot>();
                int numStored = s.StoreItemFrame(this);
                if (numStored == inventoryItem.quantity)
                {
                    DestroySelf();
                    return;
                }
                else
                {
                    inventoryItem.quantity -= numStored;
                    break;
                }
            }
        }
        ResetDrag();
        dragging = false;
    }

    private void ResetDrag()
    {
        mTransform.position = dragStartPos;
        parentSlot.RestoreChildFrame(this);
        DestroySelf();
    }

    private void DestroySelf() {
        transform.SetParent(null);
        if (parentSlot.associatedInventory != null) parentSlot.associatedInventory.ForceAlertListeners();
        Destroy(gameObject);
    }

    public void SetQuantity(int newQuantity){
        inventoryItem.quantity = newQuantity;
        quantityText.GetComponent<Text>().text = inventoryItem.quantity.ToString();
    }

    public void SetInventoryItem(InventoryItem i) {
        inventoryItem = i;
        quantityText.GetComponent<Text>().text = i.quantity.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!dragging && Input.GetButton("AutoMove")){
            if (parentSlot != null) {
                AutoMoveTarget target = parentSlot.autoMoveTarget;
                if (target != null && target.type != AutoMoveTarget.AutomoveType.None)
                {
                    int storedQuantity = 0;
                    switch (target.type)
                    {
                        case AutoMoveTarget.AutomoveType.TargetInventory:
                            storedQuantity = target.associatedInventory.AddMaxOf(inventoryItem);
                            break;
                        case AutoMoveTarget.AutomoveType.TargetSlot:
                            storedQuantity = target.associatedSlot.StoreItemFrame(this);
                            break;
                    }
                    if (storedQuantity >= inventoryItem.quantity)
                    {
                        parentSlot.DestroyItemFrame();
                    }
                    else 
                    {
                        SetQuantity(inventoryItem.quantity - storedQuantity);
                    }
                }
            }
        }
    }
}
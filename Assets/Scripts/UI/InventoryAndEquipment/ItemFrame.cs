using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public InventoryItem m_InventoryItem;

    public GameObject nameText;
    public GameObject quantityText;

    public Image itemFrameImage;
    public Color baseColour;
    public Color onHighlightColour;
    private Image backgroundImage;

    private RectTransform mTransform;
    private Vector2 dragStartPos;
    public Slot parentSlot;

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

        nameText.GetComponent<Text>().text = m_InventoryItem.prettyName;
        quantityText.GetComponent<Text>().text = m_InventoryItem.quantity.ToString();
        itemFrameImage.sprite = m_InventoryItem.inventorySprite;
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
            if (result.gameObject.GetComponent<Slot>() != null) {
                Slot s = result.gameObject.GetComponent<Slot>();
                int numStored = s.StoreItemFrame(this);
                if (numStored == m_InventoryItem.quantity)
                {
                    DestroySelf();
                    return;
                }
                else {
                    m_InventoryItem.quantity -= numStored;
                    break;
                }
            }
        }
        ResetDrag();
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
        m_InventoryItem.quantity = newQuantity;
        quantityText.GetComponent<Text>().text = m_InventoryItem.quantity.ToString();
    }

    public void SetInventoryItem(InventoryItem i) {
        m_InventoryItem = i;
        quantityText.GetComponent<Text>().text = i.quantity.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetButton("AutoMove")){
            if (parentSlot != null) {
                AutoMoveTarget target = parentSlot.autoMoveTarget;
                if (target != null && target.type != AutoMoveTarget.AutomoveType.None)
                {
                    int storedQuantity = 0;
                    switch (target.type)
                    {
                        case AutoMoveTarget.AutomoveType.TargetInventory:
                            storedQuantity = target.associatedInventory.AddMaxOf(m_InventoryItem);
                            break;
                        case AutoMoveTarget.AutomoveType.TargetSlot:
                            storedQuantity = target.associatedSlot.StoreItemFrame(this);
                            break;
                    }
                    Debug.Log("Automoving item, quantity moved: " + storedQuantity);
                    if (storedQuantity >= m_InventoryItem.quantity)
                    {
                        parentSlot.DestroyItemFrame();
                    }
                    else 
                    {
                        SetQuantity(m_InventoryItem.quantity - storedQuantity);
                    }
                }
            }
        }
    }
}
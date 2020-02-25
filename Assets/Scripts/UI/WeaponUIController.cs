using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIController : MonoBehaviour
{
    public GameObject contents;
    public GameObject weaponFrame;
    public GameObject itemFrame;

    private RectTransform contentsRect;
    private GameObject playerShip;

    private void Awake()
    {
        contentsRect = contents.GetComponent<RectTransform>();
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }

    public void UpdateContents() {
        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        foreach(Transform child in playerShip.transform) {
            if (child.CompareTag("Hull")) {
                foreach (Transform potentialWeaponAttachment in child) {
                    if (potentialWeaponAttachment.CompareTag("WeaponAttachment")) { //find all weapon attachments
                        //spawn an equip slot
                        Transform weaponAttachment = potentialWeaponAttachment;
                        GameObject equipSlot = Instantiate(weaponFrame, contents.transform);

                        RectTransform equipRect = equipSlot.GetComponent<RectTransform>();
                        equipRect.localPosition = new Vector2(x * equipRect.rect.width, y * equipRect.rect.height);
                        equipSlot.transform.Find("SlotText").GetComponent<Text>().text = weaponAttachment.name;
                        //if a weapon is equiped on the player ship
                        if (weaponAttachment.childCount > 0)
                        {
                            GameObject weaponObject = weaponAttachment.GetChild(0).gameObject;
                            GameObject screenItem = Instantiate(itemFrame, equipRect);
                            screenItem.transform.SetAsFirstSibling();

                            GameObject itemSprite = screenItem.transform.Find("ItemSprite").gameObject;
                            itemSprite.transform.Find("QuantityText").GetComponent<Text>().text = "";
                            itemSprite.transform.Find("NameText").GetComponent<Text>().text = weaponObject.GetComponent<Weapon>().prettyName;
                            itemSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Weapons/" + weaponObject.name);

                            ItemFrame frame = screenItem.GetComponent<ItemFrame>();
                            frame.itemName = weaponObject.name;
                            frame.itemQuantity = 1;
                        }
                        x++;
                        if (x * equipRect.rect.width >= contentsRect.rect.width)
                        {
                            x = 0;
                            y--;
                        }
                    }
                }
                break;
            }
        }
    }

    private void OnEnable()
    {
        UpdateContents();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotLayoutController : MonoBehaviour
{
    public GameObject weaponFramePrefab;
    public RectTransform contents;
    private GameObject playerShip;

    private void Awake()
    {
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }

    public void SetupSlots() {
        foreach (Transform child in contents)
        {
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        foreach(Transform child in playerShip.transform) {
            if (child.CompareTag("Hull")) {
                foreach (Transform weaponAttachment in child) {
                    if (weaponAttachment.CompareTag("WeaponAttachment")) { //if it actually is a weapon attachment point
                        //spawn an equip slot
                        GameObject equipSlot = Instantiate(weaponFramePrefab, contents.transform);
                        equipSlot.GetComponent<EquipSlot>().slotText.text = weaponAttachment.name;
                        RectTransform equipRect = equipSlot.GetComponent<RectTransform>();
                        equipRect.localPosition = new Vector2(x * equipRect.rect.width, y * equipRect.rect.height);
                        //if a weapon is equiped on the player ship
                        if (weaponAttachment.childCount > 0)
                        {
                            Weapon weapon = weaponAttachment.GetChild(0).gameObject.GetComponent<Weapon>();
                            equipSlot.GetComponent<EquipSlot>().TryAddItemFrameFor(weapon.m_inventoryItem);
                        }
                        x++;
                        if (x * equipRect.rect.width >= contents.rect.width)
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

    void OnEnable()
    {
        SetupSlots();
    }
}

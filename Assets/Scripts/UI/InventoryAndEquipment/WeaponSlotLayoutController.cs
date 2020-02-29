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
                        EquipSlot equipSlot = Instantiate(weaponFramePrefab, contents.transform).GetComponent<EquipSlot>();
                        equipSlot.slotText.text = weaponAttachment.name;
                        equipSlot.physicalAttachmentPoint = weaponAttachment;

                        if (weaponAttachment.childCount > 0)
                        {
                            equipSlot.SpawnItemFrame(weaponAttachment.GetChild(0).GetComponent<Weapon>().m_inventoryItem);
                        }

                        RectTransform equipRect = equipSlot.GetComponent<RectTransform>();
                        equipRect.localPosition = new Vector2(x * equipRect.rect.width, y * equipRect.rect.height);

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

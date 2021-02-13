using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeaponSlots : MonoBehaviour
{
    public GameObject slotPrefab;
    private GameObject playerShip;

    public AutoMoveTarget autoMoveTarget;

    private void Awake()
    {
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        GameObject playerShipHull = null;
        foreach (Transform child in playerShip.transform) if (child.CompareTag("Hull")) playerShipHull = child.gameObject;

        if (playerShipHull != null)
        {
            foreach (Transform child in playerShipHull.transform)
            {
                if (child.CompareTag("WeaponAttachment")) {
                    Slot s = Instantiate(slotPrefab, transform).GetComponent<Slot>();
                    s.associatedEquipPoint = child;
                    s.autoMoveTarget = autoMoveTarget;
                    s.equipType = EquipType.Weapon;
                    s.CreateFrameForEquipPoint();
                }
            }
        }
    }
}

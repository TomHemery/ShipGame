using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIController : MonoBehaviour
{
    public GameObject contents;
    public GameObject weaponFrame;

    private RectTransform contentsRect;
    private GameObject playerShip;

    private void Awake()
    {
        contentsRect = contents.GetComponent<RectTransform>();
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }

    public void UpdateContents() {
        List<GameObject> weapons = new List<GameObject>();
        foreach(Transform child in playerShip.transform) {
            if (child.CompareTag("Hull")) {
                foreach (Transform grandChild in child) {
                    if (grandChild.CompareTag("WeaponAttachment")) {
                        if (grandChild.childCount > 0) weapons.Add(grandChild.GetChild(0).gameObject);
                    }
                }
                break;
            }
        }

        foreach (GameObject weaponObject in weapons) { 
            
        }
    }

    private void OnEnable()
    {
        UpdateContents();
    }
}

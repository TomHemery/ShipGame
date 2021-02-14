using UnityEngine;
using UnityEngine.UI;

public class CreateShipRepresentation : MonoBehaviour
{
    private Image image;

    public GameObject slotPrefab;
    public AutoMoveTarget autoMoveTarget;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        GameObject playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        SpriteRenderer renderer = playerShip.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        image.sprite = sprite;

        GameObject playerShipHull = null;
        foreach(Transform child in playerShip.transform) if (child.CompareTag("Hull")) playerShipHull = child.gameObject;
        
        if(playerShipHull != null)
        {
            foreach (Transform child in playerShipHull.transform)
            {
                if (child.CompareTag("WeaponAttachment"))
                {
                    GameObject slotObject = Instantiate(slotPrefab, transform);
                    slotObject.GetComponent<RectTransform>().anchoredPosition = child.localPosition * 80;
                    Slot s = slotObject.GetComponent<Slot>();

                    s.associatedEquipPoint = child;
                    s.autoMoveTarget = autoMoveTarget;
                    s.equipType = EquipType.Weapon;
                    s.CreateFrameForEquipPoint();
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}

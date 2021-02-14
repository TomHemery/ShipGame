using UnityEngine;

public class HullSpawner : MonoBehaviour
{
    public const string DEFAULT_HULL = "MiningShip";
    public static readonly string[] DEFAULT_WEAPONS = {
        "MiningLaser",
        "MiningLaser"
    };

    [HideInInspector]
    public string hull;
    [HideInInspector]
    public string[] weapons;

    public void DestroyHull() {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        transform.DetachChildren();
    }

    public void SpawnHull() {
        SpawnHull(hull, weapons);   
    }

    public void SpawnDefaultHull() {
        SpawnHull(DEFAULT_HULL, DEFAULT_WEAPONS);
    }

    private void SpawnHull(string hull, string[] weapons) {
        
        if (transform.childCount > 0) DestroyHull();

        GameObject ship = Instantiate(PrefabDatabase.Get(hull), transform);
        if (weapons != null && weapons.Length > 0)
        {
            int index = 0;
            foreach (Transform child in ship.transform)
            {
                if (child.gameObject.CompareTag("WeaponAttachment"))
                {
                    if (index < weapons.Length)
                    {
                        Instantiate(PrefabDatabase.Get(weapons[index]), child.transform);
                    }
                    else
                        break;
                    index++;
                }
            }
        }
    }
}

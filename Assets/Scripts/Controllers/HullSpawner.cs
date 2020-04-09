using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullSpawner : MonoBehaviour
{
    public string hull;
    public string[] weapons;

    public void SpawnHull() {
        GameObject ship = Instantiate(PrefabDatabase.PrefabDictionary[hull], transform);
        if (weapons != null && weapons.Length > 0)
        {
            int index = 0;
            foreach (Transform child in ship.transform)
            {
                if (child.gameObject.CompareTag("WeaponAttachment"))
                {
                    if (index < weapons.Length)
                        Instantiate(PrefabDatabase.PrefabDictionary[weapons[index]], child.transform);
                    else
                        break;
                    index++;
                }
            }
        }
    }
}

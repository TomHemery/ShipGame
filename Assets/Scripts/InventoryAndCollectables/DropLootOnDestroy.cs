using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLootOnDestroy : MonoBehaviour
{
    [SerializeField]
    public List<LootDrop> lootDrops;
    public float spawnRange = 1.0f;

    private void OnDestroy()
    {
        foreach (LootDrop ld in lootDrops) {
            if (Random.value <= ld.dropProbability) {
                int numDrops = Random.Range(ld.dropQuantityMin, ld.dropQuantityMax + 1);
                for (int i = 0; i < numDrops; i++) {
                    Vector2 pos = transform.position;
                    pos += Random.insideUnitCircle.normalized * Random.Range(0, spawnRange);
                    Instantiate(ld.dropPrefab, pos, Quaternion.identity);
                }
            }
        }
    }

}

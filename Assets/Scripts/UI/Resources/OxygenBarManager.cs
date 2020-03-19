using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBarManager : MonoBehaviour
{

    private OxygenResourceManager oxygenResource;
    public RectTransform oxygenBar;
    Vector3 oxygenBarScale = Vector3.one;

    private void Awake()
    {
        oxygenResource = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<OxygenResourceManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (oxygenBar != null && oxygenResource != null)
        {
            oxygenBarScale.x = oxygenResource.Oxygen.Map(0, oxygenResource.MaxOxygenCapacity, 0, 1);
            oxygenBar.localScale = oxygenBarScale;
        }
    }
}

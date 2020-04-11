using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBarManager : MonoBehaviour
{

    private OxygenResource oxygenResource;
    public RectTransform oxygenBar;
    Vector3 oxygenBarScale = Vector3.one;

    private void Awake()
    {
        oxygenResource = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<OxygenResource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (oxygenBar != null && oxygenResource != null)
        {
            oxygenBarScale.x = oxygenResource.Value.Map(0, oxygenResource.MaxValue, 0, 1);
            oxygenBar.localScale = oxygenBarScale;
        }
    }
}

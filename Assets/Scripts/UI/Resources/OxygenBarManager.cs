using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBarManager : MonoBehaviour
{

    private OxygenResource oxygenResource;
    public RectTransform oxygenBar;
    public GameObject alertImage;

    Vector3 oxygenBarScale = Vector3.one;

    public Color highO2Colour;
    public Color lowO2Colour;

    bool showingWarning = false;

    private void Awake()
    {
        oxygenResource = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<OxygenResource>();
        if(alertImage != null) { alertImage.SetActive(false); }

        if (oxygenBar != null && oxygenResource != null) {
            oxygenBar.GetComponent<Image>().color = highO2Colour;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oxygenBar != null && oxygenResource != null)
        {
            oxygenBarScale.x = oxygenResource.Value.Map(0, oxygenResource.MaxValue, 0, 1);
            oxygenBar.localScale = oxygenBarScale;

            if (!showingWarning && oxygenResource.Value < oxygenResource.MaxValue / 4)
            {
                if (alertImage != null) alertImage.SetActive(true);
                oxygenBar.GetComponent<Image>().color = lowO2Colour;
                showingWarning = true;
            }
            else if(showingWarning && oxygenResource.Value >= oxygenResource.MaxValue / 4){
                oxygenBar.GetComponent<Image>().color = highO2Colour;
                if (alertImage != null) alertImage.SetActive(false);
                showingWarning = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{

    private Light myLight;
    private float lightMinRange;
    public AnimationCurve flickerCurve;

    private void Awake()
    {
        myLight = GetComponent<Light>();
        lightMinRange = myLight.range;
    }

    // Update is called once per frame
    void Update()
    {
        myLight.range = lightMinRange + flickerCurve.Evaluate(Time.time % flickerCurve.length);
    }
}

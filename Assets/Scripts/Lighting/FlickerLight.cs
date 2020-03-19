using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{

    private Light m_light;
    private float lightMinRange;
    public AnimationCurve flickerCurve;

    private void Awake()
    {
        m_light = GetComponent<Light>();
        lightMinRange = m_light.range;
    }

    // Update is called once per frame
    void Update()
    {
        m_light.range = lightMinRange + flickerCurve.Evaluate(Time.time % flickerCurve.length);
    }
}

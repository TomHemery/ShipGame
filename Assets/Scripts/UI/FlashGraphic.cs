using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashGraphic : MonoBehaviour
{
    public bool doFlash = true;
    public AnimationCurve flashCurve;

    private Graphic targetGraphic;

    private void Awake()
    {
        targetGraphic = gameObject.GetComponent<Graphic>();
    }

    private void Update()
    {
        if (doFlash) {
            Color c = targetGraphic.color;
            c.a = flashCurve.Evaluate(Time.time);
            targetGraphic.color = c;
        }
    }

}

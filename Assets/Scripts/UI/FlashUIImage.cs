using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashUIImage : MonoBehaviour
{
    public bool doFlash = true;
    public AnimationCurve flashCurve;

    private Image targetImage;

    private void Awake()
    {
        targetImage = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (doFlash) {
            Color c = targetImage.color;
            c.a = flashCurve.Evaluate(Time.time);
            targetImage.color = c;
        }
    }

}

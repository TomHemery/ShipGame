using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutOnEnable : MonoBehaviour
{
    public AnimationCurve fadeCurve;

    private Text targetText;
    private float timer = 0.0f;
    private Color startColour;

    private void Awake()
    {
        targetText = GetComponent<Text>();
        startColour = targetText.color;
    }

    private void OnEnable()
    {
        timer = 0.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        Color c = startColour;
        c.a = fadeCurve.Evaluate(timer);
        targetText.color = c;

        if (timer > fadeCurve[fadeCurve.length - 1].time) {
            gameObject.SetActive(false);
        }
    }
}

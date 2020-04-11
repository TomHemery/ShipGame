using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBar : MonoBehaviour
{
    public JumpResource resource;
    public RectTransform jumpBarGraphic;
    Vector3 barScale = Vector3.one;

    // Update is called once per frame
    void Update()
    {
        if (jumpBarGraphic != null && resource != null)
        {
            barScale.x = resource.Value.Map(0, resource.MaxValue, 0, 1);
            jumpBarGraphic.localScale = barScale;
        }
    }
}
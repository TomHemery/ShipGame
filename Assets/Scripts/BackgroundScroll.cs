using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float ScrollSpeedModifier;
    public ShipController shipController;
    Material mat;

    private void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.mainTextureOffset = new Vector2(ScrollSpeedModifier * shipController.pos.x,
            ScrollSpeedModifier * shipController.pos.y);
    }
}

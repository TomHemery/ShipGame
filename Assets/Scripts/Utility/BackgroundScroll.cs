using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float ScrollSpeedModifier;
    public ShipController playerShipContoller;
    Material mat;

    private void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.mainTextureOffset = new Vector2(ScrollSpeedModifier * playerShipContoller.M_Rigidbody.position.x,
            ScrollSpeedModifier * playerShipContoller.M_Rigidbody.position.y);
    }
}

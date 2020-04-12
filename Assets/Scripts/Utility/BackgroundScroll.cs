using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float ScrollSpeedModifier;
    [SerializeField]
    private Texture activeTexture;

    private ShipController playerShipContoller;
    Material mat;

    private float startZ;

    private void Awake()
    {
        playerShipContoller = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<ShipController>();
        mat = gameObject.GetComponent<Renderer>().material;
        startZ = transform.position.z;
    }

    private void Start()
    {
        mat.mainTexture = activeTexture;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = Camera.main.transform.position;
        pos.z = startZ;
        transform.position = pos;

        mat.mainTextureOffset = new Vector2(ScrollSpeedModifier * playerShipContoller.M_Rigidbody.position.x,
            ScrollSpeedModifier * playerShipContoller.M_Rigidbody.position.y);
    }

    public void SetActiveTexture(Texture t) {
        activeTexture = t;
        mat.mainTexture = t;
    }

}

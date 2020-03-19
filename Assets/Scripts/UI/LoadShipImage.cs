using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadShipImage : MonoBehaviour
{
    private Image m_image;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        GameObject playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        SpriteRenderer renderer = playerShip.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        m_image.sprite = sprite;
    }
}

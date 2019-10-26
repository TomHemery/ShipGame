using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteeringControlsOneFinger : MonoBehaviour
{
    public ShipController playerShipController;
    public Canvas UICanvas;

    private Image img;
    private RectTransform rectTransform;

    Color touchedColour = new Color(1, 1, 1, 1);
    Color releasedColour = new Color(1, 1, 1, 0.6f);
    bool beingTouched = false;

    private readonly Vector2 origin = Vector2.zero;
    private readonly Vector2 vertical = new Vector2(0, -1);
    private Vector2 touchPoint = Vector2.zero;

    private float angle;
    private float rotationDampening = 0.04f;

    public float InnerRadius;
    private float OuterRadius;

    private void Start()
    {
        img = gameObject.GetComponent<Image>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        OuterRadius = rectTransform.rect.width * UICanvas.scaleFactor;
    }

    // Update is called once per frame
    void Update()
    {
        beingTouched = false;
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                img.color = touchedColour;
                beingTouched = true;

                touchPoint = touch.position;
                touchPoint.x -= transform.position.x;
                touchPoint.y -= transform.position.y;
                break;
            }
        }

        //DEBUG
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                img.color = touchedColour;
                beingTouched = true;
                touchPoint = Input.mousePosition;
                touchPoint.x -= transform.position.x;
                touchPoint.y -= transform.position.y;
            }
        }
#endif

        if (beingTouched)
        {
            float dist = Vector2.Distance(origin, touchPoint);
            if (OuterRadius >= dist && InnerRadius <= dist)
            {
                playerShipController.SetTargetSpeed(touchPoint.x, InnerRadius, OuterRadius);
                angle = Vector2.Angle(vertical, touchPoint) - 90;
                playerShipController.SetRotation(angle * rotationDampening);
            }
            else
                beingTouched = false;
        }
        if(!beingTouched)
        {
            img.color = releasedColour;
            playerShipController.SetRotation(0);
        }
    }
}

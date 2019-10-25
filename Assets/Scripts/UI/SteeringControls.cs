using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteeringControls : MonoBehaviour
{

    Vector2 touchStart = new Vector2();
    Vector2 touchEnd = new Vector2();
    Vector2 outputLine = new Vector2();
    Vector2 vertical = new Vector2(0, -1);
    Vector2 temp;

    float angle;
    float maxAngle = 51.0f;
    float rotationDampening = 0.16f;

    int maxX = 0;
    int minX = 0;

    public ControllerPlayerShip shipController;
    private Image img;

    Color touchedColour = new Color(1, 1, 1, 1);
    Color releasedColour = new Color(1, 1, 1, 0.6f);

    private void OnEnable()
    {
        maxX = Screen.height + Screen.width / 10;
        minX = Screen.height / 4;
    }

    private void Start()
    {
        img = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount >= 2)
        {
            touchStart = Input.GetTouch(0).position;
            touchEnd = Input.GetTouch(1).position;
            if (touchEnd.x < touchStart.x)
            {
                temp = touchStart;
                touchStart = touchEnd;
                touchEnd = temp;
            }

            outputLine = new Vector2(touchStart.x - touchEnd.x, touchStart.y - touchEnd.y);
            outputLine.Set(-outputLine.y, outputLine.x);

            if (outputLine.magnitude > maxX) outputLine = VectorMethods.SetMagnitude(outputLine, maxX);
            else if (outputLine.magnitude < minX) outputLine = VectorMethods.SetMagnitude(outputLine, minX);
            shipController.SetTargetSpeed(outputLine.magnitude, minX, maxX);

            angle = Vector2.Angle(vertical, outputLine);
            if (touchStart.y > touchEnd.y) { angle = -angle; }
            if (angle > maxAngle) { angle = maxAngle; }
            else if (angle < -maxAngle) { angle = -maxAngle; }
            shipController.SetRotation(angle * rotationDampening);
            img.color = touchedColour;
        }
        else {
            shipController.SetRotation(0);
            img.color = releasedColour;
        }
    }
}

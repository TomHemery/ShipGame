using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShipController : ShipController
{
    [HideInInspector]
    public bool RespondToInput = true;
    [HideInInspector]
    public bool StoryControlOverride = false;

    private void Update()
    {
        if (RespondToInput && !StoryControlOverride)
        {
            //Acceleration based on user input 
            thrustMode = Input.GetButton("Accelerate") ? ThrustMode.Forward :
                         Input.GetButton("Decelerate") ? ThrustMode.Backward :
                         ThrustMode.None;

            if (Camera.main != null)
            {
                // convert mouse position into world coordinates
                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // get direction you want to point at
                Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;

                // set vector of transform directly
                desiredRotation = direction;

                if (Input.GetButtonDown("Fire"))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                        foreach (Weapon w in weapons) w.EnableAutoFire();
                }
                else if (Input.GetButtonUp("Fire"))
                {
                    foreach (Weapon w in weapons) w.DisableAutoFire();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : ShipController
{

    public List<Weapon> playerShipWeapons;

    private void Update()
    {
        //Acceleration based on user input 
        enableThrust = Input.GetButton("Accelerate");

        // convert mouse position into world coordinates
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;

        // set vector of transform directly
        transform.right = direction;

        if (Input.GetButtonDown("Fire"))
        {
            foreach (Weapon w in playerShipWeapons) w.DoAutoFire = true;
        }
        else if (Input.GetButtonUp("Fire"))
        {
            foreach (Weapon w in playerShipWeapons) w.DoAutoFire = false;
        }
    }
}

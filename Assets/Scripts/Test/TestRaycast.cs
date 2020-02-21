using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D front = Physics2D.Raycast(transform.position, transform.up);
        if (front.collider != null) Debug.Log(front.collider.gameObject);
    }
}

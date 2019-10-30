using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayVertical : MonoBehaviour
{

    Vector3 startEuler;

    void Start()
    {
        startEuler = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = startEuler;
    }
}

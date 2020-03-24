using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour
{

    private void Update()
    {
        if(!GameManager.SimPaused) updateResource();
    }

    public abstract void updateResource();
}

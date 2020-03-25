using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour
{

    protected virtual void Update()
    {
        if(!GameManager.SimPaused) updateResource();
    }

    public abstract void updateResource();
}

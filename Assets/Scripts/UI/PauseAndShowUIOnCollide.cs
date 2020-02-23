using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAndShowUIOnCollide : PauseAndShowUI
{
    private bool sleepBehaviour = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!sleepBehaviour && collision.gameObject.CompareTag("PlayerShip"))
        {
            PauseScene();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (sleepBehaviour) sleepBehaviour = false;
    }

    public override void UnPauseScene()
    {
        base.UnPauseScene();
        sleepBehaviour = true;
    }
}

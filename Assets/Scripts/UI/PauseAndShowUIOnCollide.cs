using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAndShowUIOnCollide : PauseAndShowUI
{
    private bool sleepBehaviour = false;
    public bool behaviourEnabled = true;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (behaviourEnabled && !sleepBehaviour && collision.gameObject.CompareTag("PlayerShip"))
        {
            PauseAndShow();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (sleepBehaviour) sleepBehaviour = false;
    }

    public override void UnPauseAndHide()
    {
        base.UnPauseAndHide();
        sleepBehaviour = true;
    }

    public void PauseBehaviourUntilCollisionExit() {
        sleepBehaviour = true;
    }
}

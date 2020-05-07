using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningStationUIToggle : PauseAndShowUI
{
    private bool sleepBehaviour = false;
    private bool behaviourEnabled = true;
    public ToggleTargetOnKeypress playerInventoryToggle;

    public Light[] indicatorLights;
    public Color enabledColour;
    public Color disabledColour;

    protected override void Awake()
    {
        base.Awake();

        foreach (Light light in indicatorLights)
        {
            light.color = enabledColour;
        }
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
        playerInventoryToggle.ForceHideTarget();
        sleepBehaviour = true;
    }

    public override void SilentUnPauseAndHide()
    {
        base.SilentUnPauseAndHide();
        playerInventoryToggle.ForceHideTarget();
        sleepBehaviour = true;
    }

    public void PauseBehaviourUntilCollisionExit() {
        sleepBehaviour = true;
    }

    public void SetBehaviourEnabled(bool active) {
        behaviourEnabled = active;
        foreach (Light light in indicatorLights) {
            if (active) light.color = enabledColour;
            else light.color = disabledColour;
        }
    }
}

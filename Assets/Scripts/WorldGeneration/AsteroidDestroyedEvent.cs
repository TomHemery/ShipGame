using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AsteroidDestroyedEvent : MonoBehaviour
{
    public static UnityEvent OnDestroyEvent = new UnityEvent();

    private void Awake()
    {
        GetComponent<HealthResource>().OnExploded += AsteroidDestroyedEvent_OnExploded;
    }

    private void AsteroidDestroyedEvent_OnExploded(object sender, System.EventArgs e)
    {
        OnDestroyEvent.Invoke();
    }
}
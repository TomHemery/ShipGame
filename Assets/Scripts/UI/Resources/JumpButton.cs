using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpButton : MonoBehaviour
{
    public Resource jumpResource;

    private void Awake()
    {
        jumpResource.ResourceValueChangedEvent += OnResourceChanged;
        gameObject.SetActive(false);
    }

    private void OnResourceChanged(object sender, ResourceChangedEventArgs e)
    {
        if (e.NewValue >= e.MaxValue) {
            gameObject.SetActive(true);
        }
    }

    public void OnJumpButtonPressed() {

    }
}

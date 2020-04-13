using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTargetOnKeypress : MonoBehaviour
{
    public GameObject target;
    public string key;
    public bool Show { get; private set; } = false;

    private void Start()
    {
        target.SetActive(Show);
    }

    private void Update()
    {
        if (Input.GetButtonUp(key)) {
            Show = !Show;
            target.SetActive(Show);
        }
    }

    public void ForceHideTarget() {
        Show = false;
        target.SetActive(false);
    }

    public void ForceShowTarget()
    {
        Show = true;
        target.SetActive(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTargetOnKeypress : MonoBehaviour
{
    public GameObject target;
    public string key;
    public bool show = false;

    private void Start()
    {
        target.SetActive(show);
    }

    private void Update()
    {
        if (Input.GetButtonUp(key)) {
            show = !show;
            target.SetActive(show);
        }
    }
}
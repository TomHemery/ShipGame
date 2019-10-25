using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Framerate : MonoBehaviour
{

    private Text framerateText;

    // Start is called before the first frame update
    void Start()
    {
        framerateText = GetComponent<Text>();           
    }

    // Update is called once per frame
    void Update()
    {
        framerateText.text = "" + 1.0f / Time.deltaTime;
    }
}

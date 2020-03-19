using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPanel : MonoBehaviour
{
    public RectTransform highlightObject;
    public float offset = -20;
    public int Index { get; private set; } = 0;
    public int MaxIndex = 3;

    bool inputFlag = false;

    void Update()
    {
        if (!inputFlag && Input.GetAxis("Vertical") < 0)
        {
            SetIndex(Index < MaxIndex ? Index + 1 : 0);
            inputFlag = true;
        }
        else if (!inputFlag && Input.GetAxis("Vertical") > 0)
        {
            SetIndex(Index > 0 ? Index - 1 : MaxIndex);
            inputFlag = true;
        }
        else if (inputFlag && Input.GetAxis("Vertical") == 0)
        {
            inputFlag = false;
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        SetIndex(0);
    }

    public void SetIndex(int i)
    {
        Index = i;
        highlightObject.anchoredPosition = new Vector3(highlightObject.anchoredPosition.x, Index * offset);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayout : MonoBehaviour
{
    public GridLayoutGroup targetLayout;
    public int columnCount;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCoroutine(Resize());
    }

    private IEnumerator Resize()
    {
        yield return new WaitForEndOfFrame();
        float x = rectTransform.rect.width / columnCount;
        targetLayout.cellSize = new Vector2(x, x);
    }
}
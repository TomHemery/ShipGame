using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayout : MonoBehaviour
{
    public GridLayoutGroup targetLayout;
    public int columnCount;
    private int padding = 5;

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
        float sideLength = (rectTransform.rect.width - (columnCount - 1) * padding) / columnCount;
        targetLayout.cellSize = new Vector2(sideLength, sideLength);
        //targetLayout.padding = new RectOffset(padding, padding, padding, padding);
        targetLayout.spacing = new Vector2(padding, padding);
    }
}
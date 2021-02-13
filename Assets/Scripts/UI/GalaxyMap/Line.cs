using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private RectTransform rectTransform;

    public void Set(Vector3 start, Vector3 end, float lineWidth, Canvas targetCanvas) {
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
        Vector3 differenceVector = end - start;
        rectTransform.sizeDelta = new Vector2(differenceVector.magnitude / targetCanvas.scaleFactor, lineWidth);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.position = start;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
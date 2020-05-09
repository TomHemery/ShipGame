using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private RectTransform m_rectTransform;

    public void Set(Vector3 start, Vector3 end, float lineWidth, Canvas targetCanvas) {
        if(m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();
        Vector3 differenceVector = end - start;
        m_rectTransform.sizeDelta = new Vector2(differenceVector.magnitude / targetCanvas.scaleFactor, lineWidth);
        m_rectTransform.pivot = new Vector2(0, 0.5f);
        m_rectTransform.position = start;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        m_rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    private float length = 0;
    private float lengthChangeRate = 20f;
    public float range = 10;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
    }

    public void OnDisable()
    {
        length = 0;
        m_renderer.size = new Vector2(m_renderer.size.x, length);
    }

    private void Update()
    {
        float targetLength = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        targetLength = targetLength < range ? targetLength : range;

        if (length < targetLength) length += Time.deltaTime * lengthChangeRate;
        if (length > targetLength) length = targetLength;

        Debug.Log(targetLength + ", " + length);

        m_renderer.size = new Vector2(m_renderer.size.x, length);
    }
}

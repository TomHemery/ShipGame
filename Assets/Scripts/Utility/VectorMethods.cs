using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorMethods
{
    public static Vector2 FromRadians(float a) {
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    public static Vector2 FromDegrees(float d) { 
        return FromRadians(d * Mathf.Deg2Rad);
    }
    public static Vector2 SetMagnitude(Vector2 v, float m)
    {
        v.Normalize();
        v.Set(v.x * m, v.y * m);
        return v;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFollowTarget : MonoBehaviour
{

    public Transform Target;
    public Vector3 Offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        transform.position = Target.position + Offset;
    }
}

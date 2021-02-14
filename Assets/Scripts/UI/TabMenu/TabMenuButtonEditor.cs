using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabMenuButton))]
public class TabMenuButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
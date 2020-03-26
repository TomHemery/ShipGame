using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationDatabase : MonoBehaviour
{
    public static AnimationDatabase Instance { get; private set; } = null;
    public static Dictionary<string, RuntimeAnimatorController> AnimatorDictionary { get; private set; } = new Dictionary<string, RuntimeAnimatorController>();

    [SerializeField]
    private RuntimeAnimatorController[] allAnimators;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        foreach (RuntimeAnimatorController a in allAnimators)
        {
            AnimatorDictionary.Add(a.name, a);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueDatabase : MonoBehaviour
{

    public static DialogueDatabase Instance { get; private set; } = null;
    public static Dictionary<string, TextAsset> DialogueDictionary { get; private set; } = new Dictionary<string, TextAsset>();

    [SerializeField]
    private TextAsset[] allDialogues;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        foreach (TextAsset t in allDialogues)
        {
            DialogueDictionary.Add(t.name, t);
        }
    }
}

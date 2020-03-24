using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseButtonBehaviour : MonoBehaviour
{
    public int targetIndex;
    private string responseText;

    public void SetPromptText(string t) {
        responseText = t;
        GetComponentInChildren<Text>().text = t;
    }

    public void OnButtonClick() {
        DialoguePanel.MainDialoguePanel.OnResponseSelected(responseText, targetIndex);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    Text dialogueText;
    DialogueGraph currentDialogue = null;
    DialogueStage stage = DialogueStage.PromptPlayer;

    private List<string> currentResponses;
    private SelectionPanel selectionPanel;

    private void Awake()
    {
        dialogueText = gameObject.GetComponentInChildren<Text>();
        selectionPanel = transform.Find("SelectionPanel").GetComponent<SelectionPanel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        selectionPanel.Deactivate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Opens a new dialogue, stops player from moving
    /// </summary>
    /// <param name="dialogue">The DialogueGraph to be opened</param>
    public void OpenDialogue(DialogueGraph dialogue)
    {
        dialogue.CurrentNodeIndex = dialogue.EntryNodeIndex;
        currentDialogue = dialogue;
        dialogueText.text = GetPrompt();
        gameObject.SetActive(true);
        stage = DialogueStage.PromptPlayer;
    }

    /// <summary>
    /// clears any and all dialogue prompts and resets the current dialogue, enables player movement
    /// </summary>
    public void ClearDialogue()
    {
        dialogueText.text = "";
        currentDialogue = null;
        selectionPanel.Deactivate();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Is the dialogue open and active?
    /// </summary>
    /// <returns>true if the dialogue is being interacted with currently</returns>
    public bool DialogueOpened()
    {
        return gameObject.activeInHierarchy;
    }

    //called when the user presses the interact button
    public void OnInteract()
    {
        switch (stage)
        {
            case DialogueStage.TakeResponse: //we just got a response, now prompt the player
                int outcomeIndex = GetOutcome();
                //if the selected prompt changes the entry point of the conversation
                if (currentDialogue.GetCurrentNode().EntryPoints.TryGetValue(
                    currentResponses[selectionPanel.Index], out int newEntryPoint))
                    currentDialogue.EntryNodeIndex = newEntryPoint;
                //if the outcome is within the conversation
                if (outcomeIndex >= 0)
                {
                    currentDialogue.CurrentNodeIndex = outcomeIndex;
                    dialogueText.text = GetPrompt();
                    stage = DialogueStage.PromptPlayer;
                    selectionPanel.Deactivate();
                }
                //time to quite dialogue
                else
                {
                    ClearDialogue();
                }
                break;
            case DialogueStage.PromptPlayer: //we just prompted the user, so now we show responses
                currentResponses = currentDialogue.GetCurrentNode().GetResponseList();
                dialogueText.text = GenerateResponsePrompt(currentResponses);
                stage = DialogueStage.TakeResponse;
                selectionPanel.Activate();
                selectionPanel.MaxIndex = currentResponses.Count - 1;
                break;
        }
    }

    //returns a response prompt for the player based on the current dialogue node
    private string GenerateResponsePrompt(List<string> responseList)
    {
        string responses = "";
        int counter = 1;
        foreach (string response in responseList)
        {
            responses += counter++ + ": " + response + "\n";
        }
        return responses;
    }

    //returns a prompt for the player based on the current dialogue node
    private string GetPrompt()
    {
        return currentDialogue.GetCurrentNode().Prompt;
    }

    //returns the selected response's outcome based on the currently highlighted response from the list
    private int GetOutcome()
    {
        return currentDialogue.GetCurrentNode().Responses[currentResponses[selectionPanel.Index]];
    }

    public enum DialogueStage
    {
        PromptPlayer,
        TakeResponse
    };
}

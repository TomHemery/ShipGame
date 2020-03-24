using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour, IPointerClickHandler
{
    Text dialogueText;
    DialogueGraph currentDialogue = null;
    DialogueStage prevStage = DialogueStage.PromptPlayer;

    private List<string> currentResponses;
    public SelectionPanel selectionPanel;
    public GameObject characterPortrait;

    public static DialoguePanel MainDialoguePanel { get; private set; } = null;

    private void Awake()
    {
        dialogueText = gameObject.GetComponentInChildren<Text>();
        MainDialoguePanel = this;
        gameObject.SetActive(false);
        selectionPanel.Deactivate();
    }

    private void OnDestroy()
    {
        if (MainDialoguePanel == this) {
            MainDialoguePanel = null;
        }
    }

    /// <summary>
    /// Opens a new dialogue and pauses the game state
    /// </summary>
    /// <param name="dialogueName">The system name of the DialogueGraph to be openend</param>
    public void OpenDialogue(string dialogueName) {
        OpenDialogue(
            DialogueGraph.GenerateDialogueFromXML(DialogueDatabase.DialogueDictionary[dialogueName])
        );
    }

    /// <summary>
    /// Opens a new dialogue and pauses the game state
    /// </summary>
    /// <param name="dialogue">The DialogueGraph to be opened</param>
    public void OpenDialogue(DialogueGraph dialogue)
    {
        dialogue.CurrentNodeIndex = dialogue.EntryNodeIndex;
        currentDialogue = dialogue;
        dialogueText.text = GetPrompt();
        gameObject.SetActive(true);
        prevStage = DialogueStage.PromptPlayer;
        if (currentDialogue.RequireSimPause) GameManager.PauseSim();
    }

    /// <summary>
    /// clears any and all dialogue prompts and resets the current dialogue, enables player movement
    /// </summary>
    public void ClearDialogue()
    {
        if (currentDialogue.RequireSimPause) GameManager.UnPauseSim();
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

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (prevStage)
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
                    prevStage = DialogueStage.PromptPlayer;
                    selectionPanel.Deactivate();
                }
                //time to quite dialogue
                else
                {
                    ClearDialogue();
                }
                break;
            case DialogueStage.PromptPlayer: //we just prompted the user, so now we show responses
                if (currentDialogue.GetCurrentNode().continueWithoutResponse)
                {
                    int nextIndex = currentDialogue.GetCurrentNode().nextIndexOnContinue;
                    if (nextIndex >= 0)
                    {
                        currentDialogue.CurrentNodeIndex = nextIndex;
                        dialogueText.text = GetPrompt();
                        prevStage = DialogueStage.PromptPlayer;
                        selectionPanel.Deactivate();
                    }
                    else {
                        ClearDialogue();
                    }
                }
                else
                {
                    currentResponses = currentDialogue.GetCurrentNode().GetResponseList();
                    dialogueText.text = GenerateResponsePrompt(currentResponses);
                    prevStage = DialogueStage.TakeResponse;
                    selectionPanel.Activate();
                    selectionPanel.MaxIndex = currentResponses.Count - 1;
                }
                break;
        }
    }

    public enum DialogueStage
    {
        PromptPlayer,
        TakeResponse
    };
}

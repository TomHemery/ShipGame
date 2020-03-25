﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour, IPointerClickHandler
{
    Text dialogueText;
    DialogueGraph currentDialogue = null;
    DialogueStage currentStage = DialogueStage.PromptPlayer;

    public GameObject characterPortrait;

    public GameObject responsePanel;
    public GameObject responseButtonPrefab;
    public Transform responseContentTransform;

    public GameObject continueArrow;

    public static DialoguePanel MainDialoguePanel { get; private set; } = null;

    public AudioClip buttonPressSound;

    private void Awake()
    {
        dialogueText = gameObject.GetComponentInChildren<Text>();
        MainDialoguePanel = this;
        gameObject.SetActive(false);
        responsePanel.SetActive(false);
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
        currentStage = DialogueStage.PromptPlayer;
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
        responsePanel.SetActive(false);
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

    //generates response buttons in the content of the response scroll view based on the passed in node
    private void GenerateResponseButtons(DialogueNode node)
    {
        ClearResponseButtons();
        foreach (string response in node.GetResponseList())
        {
            GameObject responseButton = Instantiate(responseButtonPrefab, responseContentTransform);

            responseButton.GetComponent<ResponseButtonBehaviour>().targetIndex = currentDialogue.GetCurrentNode().ResponseTargets[response];
            responseButton.GetComponent<ResponseButtonBehaviour>().SetPromptText(response);
        }
    }

    private void ClearResponseButtons() {
        foreach (Transform child in responseContentTransform) {
            Destroy(child.gameObject);
        }
        responseContentTransform.DetachChildren();
    }

    //returns a prompt for the player based on the current dialogue node
    private string GetPrompt()
    {
        return currentDialogue.GetCurrentNode().Prompt;
    }

    //moves to next stage of dialogue based on the index of the selected response
    public void OnResponseSelected(string responseText, int targetIndex)
    {
        if (currentDialogue.GetCurrentNode().ResponseTargets.TryGetValue(
                responseText, out int newEntryPoint)){
            currentDialogue.EntryNodeIndex = newEntryPoint;
        }

        if (targetIndex >= 0)
        {
            currentDialogue.CurrentNodeIndex = targetIndex;
            dialogueText.text = GetPrompt();
            currentStage = DialogueStage.PromptPlayer;
        }
        else
        {
            ClearDialogue();
        }

        responsePanel.SetActive(false);
        continueArrow.SetActive(true);
        SoundEffectPlayer.SoundEffectSource.PlayOneShot(buttonPressSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //we are currently showing a prompt, we now need to show either responses or the next prompt
        if(currentStage == DialogueStage.PromptPlayer) { 
            if (currentDialogue.GetCurrentNode().continueWithoutResponse)
            {
                int nextIndex = currentDialogue.GetCurrentNode().nextIndexOnContinue;
                if (nextIndex >= 0)
                {
                    currentDialogue.CurrentNodeIndex = nextIndex;
                    dialogueText.text = GetPrompt();
                    currentStage = DialogueStage.PromptPlayer;
                }
                else {
                    ClearDialogue();
                }
            }
            else
            {
                responsePanel.SetActive(true);
                continueArrow.SetActive(false);
                dialogueText.text = "";
                GenerateResponseButtons(currentDialogue.GetCurrentNode());
                currentStage = DialogueStage.TakeResponse;
            }
        }
        SoundEffectPlayer.SoundEffectSource.PlayOneShot(buttonPressSound);
    }

    public enum DialogueStage
    {
        PromptPlayer,
        TakeResponse
    };
}

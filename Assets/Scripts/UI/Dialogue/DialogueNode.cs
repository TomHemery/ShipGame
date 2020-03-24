using System.Collections.Generic;

/// <summary>
/// individual "node" in a conversation - one prompt from the npc plus responses
/// </summary>
public class DialogueNode
{
    //Dictionary for the possible targets for this node based on the selected reply
    public Dictionary<string, int> ResponseTargets { get; private set; }

    //Dictionary for the possible new entry points for this conversation based on selected reply
    public Dictionary<string, int> EntryPoints { get; private set; }

    //the prompt shown 
    public string Prompt { get; private set; } = "";

    //the index of this dialogue
    public int Index { get; private set; } = -1;

    //continue without needing a response from the player? 
    public bool continueWithoutResponse = false;
    public int nextIndexOnContinue = -1;

    //the actor name assocaitedw with this prompt
    public string Actor { get; private set; } = "";

    //the dialogue graph to which this node belongs
    private readonly DialogueGraph dialogueGraph;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="g">The graph to which this node belongs</param>
    public DialogueNode(DialogueGraph g)
    {
        ResponseTargets = new Dictionary<string, int>();
        EntryPoints = new Dictionary<string, int>();
        dialogueGraph = g;
    }

    /// <summary>
    /// Adds a response to this node
    /// </summary>
    /// <param name="response"></param>
    /// <param name="nextDialogueIndex"></param>
    /// <param name="newEntryIndex"></param>
    public void AddResponse(string response, int nextDialogueIndex, int newEntryIndex = -1)
    {
        ResponseTargets.Add(response, nextDialogueIndex);
        if (newEntryIndex >= 0)
            EntryPoints.Add(response, newEntryIndex);
    }

    public void SetIndex(int i)
    { //can only be set once
        if (Index < 0) Index = i;
    }

    public void SetPrompt(string p)
    { //can only be set once
        if (Prompt == "") Prompt = p;
    }

    public List<string> GetResponseList()
    {
        List<string> responses = new List<string>();
        foreach (string response in ResponseTargets.Keys)
        {
            responses.Add(response);
        }
        return responses;
    }
}
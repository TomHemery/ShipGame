using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//class to represent the graph of a conversation within the game - generated from XML
public class DialogueGraph
{
    //the current node the conversation is on
    public int CurrentNodeIndex = 0;
    //the entry node of the conversation (where the npc will start from on interaction)
    public int EntryNodeIndex = -1;
    //a dictionary taking indexes and returning dialogue node objects
    public Dictionary<int, DialogueNode> allNodes = null;

    public bool RequireSimPause { get; private set; } = false;

    //tags and attributes for reading in XML objects
    private const string NODE_TAG = "node";
    private const string RESPONSE_TAG = "response";
    private const string PROMPT_TAG = "prompt";
    private const string CONTINUE_TAG = "continue";
    private const string CONVERSATION_TAG = "conversation";

    private const string INDEX_ATTRIBUTE = "index";
    private const string TARGET_ATTRIBUTE = "target";
    private const string ENTRY_NODE_ATTRIBUTE = "entryNode";
    private const string NEW_ENTRY_NODE_ATTRIBUTE = "newEntryNode";
    private const string REQUIRE_PAUSE_ATTRIBUTE = "requirePause";
    private const string CHARACTER_ATTRIBUTE = "character";
    private const string MUSIC_ATTRIBUTE = "music";

    //constructor is private to prevent just creating a dialogue graph - needs to be made from XML
    private DialogueGraph()
    {
        allNodes = new Dictionary<int, DialogueNode>();
    }

    /// <summary>
    /// Generates a dialogue graph from an XML file
    /// </summary>
    /// <param name="xmlFile">the xml file to parse for dialogue</param>
    /// <returns>DialogueGraph object that is a representation of the xml file</returns>
    public static DialogueGraph GenerateDialogueFromXML(TextAsset xmlFile)
    {
        DialogueGraph result = new DialogueGraph();
        XmlDocument xml = XMLParser.Parse(xmlFile);
        foreach (XmlNode node in xml.GetElementsByTagName(NODE_TAG))
        {
            DialogueNode newNode = new DialogueNode(result);
            newNode.SetIndex(int.Parse(node.Attributes[INDEX_ATTRIBUTE].Value));
            result.allNodes.Add(newNode.Index, newNode);

            //extract each response and the prompt from the node element
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name) {
                    case RESPONSE_TAG:
                        //a response that results in the conversation having a new entry point
                        XmlNode temp = childNode.Attributes[NEW_ENTRY_NODE_ATTRIBUTE];
                        if (temp != null)
                        {
                            newNode.AddResponse(
                                childNode.InnerText.Trim(), int.Parse(childNode.Attributes[TARGET_ATTRIBUTE].Value),
                                int.Parse(temp.InnerText.Trim())
                                );
                        }
                        //a standard response
                        else
                        {
                            newNode.AddResponse(
                                childNode.InnerText.Trim(), int.Parse(childNode.Attributes[TARGET_ATTRIBUTE].Value));
                        }
                        break;
                    case PROMPT_TAG:
                        newNode.SetPrompt(childNode.InnerText.Trim());
                        break;
                    case CONTINUE_TAG:
                        newNode.continueWithoutResponse = true;
                        newNode.nextIndexOnContinue = int.Parse(childNode.Attributes[TARGET_ATTRIBUTE].Value);
                        break;
                }
            }
            newNode.characterName = node.Attributes[CHARACTER_ATTRIBUTE].InnerText.Trim();

            newNode.changesMusic = node.Attributes[MUSIC_ATTRIBUTE] != null;
            foreach(XmlAttribute a in node.Attributes) Debug.Log(a.InnerText.Trim());
            if (newNode.changesMusic)
            {
                Debug.Log("New music from node: " + node.Attributes[MUSIC_ATTRIBUTE].InnerText.Trim());
                newNode.newMusicState =
                    (MusicPlayer.MusicState)System.Enum.Parse(typeof(MusicPlayer.MusicState), node.Attributes[MUSIC_ATTRIBUTE].InnerText.Trim());
                Debug.Log("New node music state: " + newNode.newMusicState);
            }
        }
        result.EntryNodeIndex = int.Parse(xml.GetElementsByTagName(CONVERSATION_TAG)[0].Attributes[ENTRY_NODE_ATTRIBUTE].InnerText.Trim());
        result.RequireSimPause = bool.Parse(xml.GetElementsByTagName(CONVERSATION_TAG)[0].Attributes[REQUIRE_PAUSE_ATTRIBUTE].InnerText.Trim());
        return result;
    }

    /// <summary>
    /// shortcut method to get the current node of the dialogue
    /// </summary>
    /// <returns>DialogueNode CurrentNode</returns>
    public DialogueNode GetCurrentNode()
    {
        return allNodes[CurrentNodeIndex];
    }

    /// <summary>
    /// shortcut method to get the entry node of the dialogue
    /// </summary>
    /// <returns>DialogueNode EntryNode</returns>
    public DialogueNode GetEntryNode()
    {
        return allNodes[EntryNodeIndex];
    }

    override
    public string ToString()
    {
        string result = "Conversation, entry node [" + EntryNodeIndex + "]";
        foreach (DialogueNode node in allNodes.Values)
        {
            result += "\n\tNode [" + node.Index + "]";
            result += "\n\t\tPrompt: " + node.Prompt;
            if (node.continueWithoutResponse)
            {
                result += "\n\t\tContinues without response to: " + node.nextIndexOnContinue;
            }
            else
            {
                foreach (string response in node.ResponseTargets.Keys)
                {
                    result += "\n\t\tResponse { " + response + "\n\t\t\tNext: " + node.ResponseTargets[response];
                    if (node.EntryPoints.ContainsKey(response))
                        result += "\n\t\t\tNew Entry Point: " + node.EntryPoints[response];
                    result += "}";
                }
            }
        }
        return result;
    }

}
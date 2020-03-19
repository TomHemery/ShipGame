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

    //tags and attributes for reading in XML objects
    private static readonly string NODE_TAG = "node";
    private static readonly string RESPONSE_TAG = "response";
    private static readonly string PROMPT_TAG = "prompt";
    private static readonly string CONVERSATION_TAG = "conversation";

    private static readonly string INDEX_ATTRIBUTE = "index";
    private static readonly string TARGET_ATTRIBUTE = "target";
    private static readonly string ENTRY_NODE_ATTRIBUTE = "entryNode";
    private static readonly string NEW_ENTRY_NODE_ATTRIBUTE = "newEntryNode";

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
            foreach (XmlNode responseOrPrompt in node.ChildNodes)
            {
                if (responseOrPrompt.Name == RESPONSE_TAG)
                {
                    //a response that results in the conversation having a new entry point
                    XmlNode temp = responseOrPrompt.Attributes[NEW_ENTRY_NODE_ATTRIBUTE];
                    if (temp != null)
                    {
                        newNode.AddResponse(
                            responseOrPrompt.InnerText.Trim(), int.Parse(responseOrPrompt.Attributes[TARGET_ATTRIBUTE].Value),
                            int.Parse(temp.InnerText.Trim())
                            );
                    }
                    //a standard response
                    else
                    {
                        newNode.AddResponse(
                            responseOrPrompt.InnerText.Trim(), int.Parse(responseOrPrompt.Attributes[TARGET_ATTRIBUTE].Value));
                    }
                }
                else if (responseOrPrompt.Name == PROMPT_TAG)
                {
                    newNode.SetPrompt(responseOrPrompt.InnerText.Trim());
                }
            }
        }
        result.EntryNodeIndex = int.Parse(xml.GetElementsByTagName(CONVERSATION_TAG)[0].Attributes[ENTRY_NODE_ATTRIBUTE].InnerText.Trim());
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
            foreach (string response in node.Responses.Keys)
            {
                result += "\n\t\tResponse { " + response + "\n\t\t\tNext: " + node.Responses[response];
                if (node.EntryPoints.ContainsKey(response))
                    result += "\n\t\t\tNew Entry Point: " + node.EntryPoints[response];
                result += "}";
            }
        }
        return result;
    }

}
using System.Collections;
using System.Xml;
using UnityEngine;

public class XMLParser 
{
    /// <summary>
    /// Converts an XML file to an XMLObject
    /// </summary>
    /// <param name="file">The XML file to be converted</param>
    /// <returns>an XMLObject representing the file</returns>
    public static XmlDocument Parse(TextAsset file)
    {
        XmlDocument document = new XmlDocument
        {
            PreserveWhitespace = false
        };
        document.LoadXml(file.text);
        return document;
    }
}
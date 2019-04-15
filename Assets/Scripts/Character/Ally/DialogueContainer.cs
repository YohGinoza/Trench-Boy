using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using System.IO;

[XmlRoot("DialogueCollection")]
public class DialogueContainer
{
    [XmlArray("Dialogues")]
    [XmlArrayItem("Dialogue")]

    public List<Dialogue> dialogues = new List<Dialogue>();
    
    public static DialogueContainer Load(string path)
    {
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(DialogueContainer));

        StringReader reader = new StringReader(_xml.text);

        DialogueContainer dialogues = serializer.Deserialize(reader) as DialogueContainer;

        reader.Close();

        return dialogues;
    }
   
}

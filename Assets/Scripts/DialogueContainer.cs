using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("DialogueCollection")]
public class DialogueContainer
{
    [XmlAttribute("name")]
    public string name;    


    public static DialogueContainer Load(string path)
    {

    }
   
}

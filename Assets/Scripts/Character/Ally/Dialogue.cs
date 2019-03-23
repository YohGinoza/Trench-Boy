using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;

public class Dialogue
{ 
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Who")]
    public int who;

    [XmlElement("Speaker")]
    public string speaker;

    [XmlElement("Type")]
    public string type;

    [XmlElement("ID")]
    public int id;

    [XmlElement("Line")]
    public int line;

    [XmlElement("Text")]
    public string text;
}
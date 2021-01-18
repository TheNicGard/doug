using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class SerializableDate
{
    // credit to https://stackoverflow.com/a/6328544/4200551 for solution
    // Used for current use
    [XmlIgnore]
    public System.DateTime date { get; set; }

    // For serialization.
    [XmlElement]
    public string serializableDate
    {
        get { return date.ToString("0:yyyy/MM/dd H:mm:ss"); }
        set { date = System.DateTime.ParseExact(value, "0:yyyy/MM/dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture); }
    }
}

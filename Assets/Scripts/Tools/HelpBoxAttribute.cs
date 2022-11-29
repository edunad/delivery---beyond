#if UNITY_EDITOR
using UnityEngine;
using System;

// Adapted from : http://diegogiacomelli.com.br/unitytips-helpbox-attribute/

public enum HelpBoxType{
    None,
    Info,
    Warning,
    Error
}

[AttributeUsage(AttributeTargets.Field)]
public class HelpBoxAttribute : PropertyAttribute
{
    public HelpBoxAttribute(string text, HelpBoxType type = HelpBoxType.Info) {
        Text = text;
        Type = type;
    }

    public string Text { get; }
    public HelpBoxType Type { get; }
}
#endif
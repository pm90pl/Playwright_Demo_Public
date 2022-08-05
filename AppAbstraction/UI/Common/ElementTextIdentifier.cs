using System;

namespace BBlog.Tests.AppAbstraction.UI.Common;

public class ElementTextIdentifier : Attribute
{
    public string Value { get; }
    public TextType Type { get; }

    public ElementTextIdentifier(string value, TextType type)
    {
        Value = value;
        Type = type;
    }
}

public enum TextType
{
    InnerText,
    Attribute
}
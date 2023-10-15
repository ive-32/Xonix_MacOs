
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class LabelAttribute : Attribute
{
    public string Label { get; set; }

    public LabelAttribute(string label) => Label = label;
}
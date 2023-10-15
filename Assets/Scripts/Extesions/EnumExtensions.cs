using System;
using System.Linq;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetLabel(this Enum value)
        => value.GetType().GetMember(value.ToString()).First().GetCustomAttribute<LabelAttribute>() is { } attribute 
            ? attribute.Label
            : string.Empty;
    
    public static bool IsGround(this Enum value)
        => value.GetType().GetMember(value.ToString()).First().GetCustomAttribute<GroundAttribute>() is
        {
            IsGround: true
        };
}
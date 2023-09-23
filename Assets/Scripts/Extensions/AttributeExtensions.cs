using System;
using System.Reflection;

public static class AttributeExtensions
{
    public static TAttribute GetEnumCustomAttributeOrDefault<TAttribute, TEnum>(this TEnum value,
        TAttribute defaultAttribute)
        where TAttribute : Attribute
        where TEnum : struct, Enum
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value);

        return !string.IsNullOrEmpty(name)
            ? enumType.GetField(name)!.GetCustomAttribute<TAttribute>()!
            : defaultAttribute;
    }
    
    /*public static bool IsGround<TEnum>(this TEnum value) where TEnum : struct, Enum
        => value.GetEnumCustomAttributeOrDefault(new GroundAttribute(false)).IsGround;*/
    
    public static bool IsGround(this TileType value) 
        => value is TileType.Border or TileType.Filled;
}
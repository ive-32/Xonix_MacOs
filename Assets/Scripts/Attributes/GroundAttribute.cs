using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GroundAttribute : Attribute
{
    public bool IsGround { get; set; }

    public GroundAttribute(bool ground)
    {
        IsGround = ground;
    }
}
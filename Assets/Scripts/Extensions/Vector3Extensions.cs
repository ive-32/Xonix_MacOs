using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Round(this Vector3 value)
        => new (Mathf.Round(value.x), Mathf.Round(value.y), Mathf.Round(value.z));

}
using System;
using UnityEngine;

[Serializable]
public class WrappedObject
{
    public GameObject gameObject;
    public float tileAlignment;
    public Vector3 defaultEulerAngles;

    public override string ToString()
    {
        return String.Format("WrappedObject({0}, alignment={1}, angles={2})", gameObject.name, tileAlignment, defaultEulerAngles);
    }
}

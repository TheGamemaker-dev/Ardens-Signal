using System;
using System.Reflection;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string hi = "hello";

    void Start()
    {
        FieldInfo hi2 = this.GetType().GetField("hi");
        var hi3 = hi2.GetValue(this);
        Debug.Log(hi3.ToString());
    }
}

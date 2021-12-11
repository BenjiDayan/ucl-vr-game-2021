using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    Material material;
    public Color brightColor = new Color(1, 1, 1, 1);
    public Color darkColor;


    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        float brightness = (Mathf.Sin(Time.realtimeSinceStartup * 2.5f) + 1) / 2f;
        material.SetColor("_Color", brightColor * brightness + darkColor * (1- brightness));
    }
}

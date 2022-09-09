using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitPanelScript : MonoBehaviour
{
    Image image;
    Color thisColor;
    float r;
    float g;
    float b;
    float a;

    float time = 0;

    float frequency = 1f;
    float amplitude = 50f;
    float Omega = 1f;
    void Start()
    {
        image = this.gameObject.GetComponent<Image>();
        thisColor = image.color;
        r = thisColor.r;
        g = thisColor.g;
        b = thisColor.b;
        a = thisColor.a;
        Omega = 2 * Mathf.PI * frequency;
        print("color" + thisColor);
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time * Omega;
        thisColor = new Color(r + (amplitude * Mathf.Sin(time) / 250f), b + (amplitude * Mathf.Sin(time) / 250f), g + (amplitude * Mathf.Sin(time) / 250f), a);
        this.gameObject.GetComponent<Image>().color = thisColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_TitleScene : MonoBehaviour
{
    float Rotate = 0.1f;
    [HideInInspector]
    public bool StartRotate = false; 
    void Start()
    {
        StartRotate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (StartRotate)
        {
            Vector3 Dir = this.gameObject.transform.localEulerAngles;
            Dir.y += Rotate;
            this.gameObject.transform.localEulerAngles = Dir;
        }
    }

    public void StartRotation()
    {
        StartRotate = true;
    }
}

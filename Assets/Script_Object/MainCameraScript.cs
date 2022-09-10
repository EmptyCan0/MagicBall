using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    float CameraShakePower = 0;
    float TimeCount = 0;
    float FinishShakeTime = 1.0f;

    bool StartCameraShake = false;

    Camera camera;
    float DefaultFieldOfView = 60;
    
    Transform Mypos;
    void Start()
    {
        Mypos = this.transform;
        camera = this.gameObject.GetComponent<Camera>();
        DefaultFieldOfView = camera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (StartCameraShake)
        {
            TimeCount += Time.deltaTime;
            CameraShake(CameraShakePower,TimeCount);
            if (TimeCount > FinishShakeTime)
            {
                StartCameraShake = false;
                camera.fieldOfView = DefaultFieldOfView;
            }
        }
    }

    private void CameraShake(float power,float time)
    {
        float Frequency = 6f;
        float Omega = 2 * Mathf.PI * Frequency;
        camera.fieldOfView = DefaultFieldOfView  + (power * Mathf.Sin(Omega * time));

    }

    public void StartCameraShaking(float Power)
    {
        StartCameraShake = true;
        TimeCount = 0;
        CameraShakePower = Power;
    }
}

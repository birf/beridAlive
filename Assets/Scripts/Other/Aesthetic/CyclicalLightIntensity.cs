using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
public class CyclicalLightIntensity : MonoBehaviour
{
    public Light2D targetLight;
    public float startingIntensity;
    [Range(0,2f)] public float minIntensity = 0f;
    [SerializeField][Range(0.01f,100.0f)] float flashesPerSecond = 0.5f;

    float curTime;
    void Awake()
    {
        targetLight = GetComponent<Light2D>();
        startingIntensity = targetLight.intensity;
    }

    void Update()
    {
        float mid = (startingIntensity - minIntensity);
        targetLight.intensity = (Mathf.Sin(curTime * flashesPerSecond) * (mid / 2)) + mid;
        targetLight.intensity = Mathf.Clamp(targetLight.intensity,minIntensity,startingIntensity);
        curTime += Time.deltaTime * Mathf.PI;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] Camera affectedCamera;
    [SerializeField] [Range(0.01f,1.0f)] float xMultiplier = 0.5f;

    void Awake()
    {
        if (!affectedCamera)
            affectedCamera = FindObjectOfType<Camera>();
    }

    void Update() 
    {
        Vector3 newPos = new Vector3(affectedCamera.transform.position.x * xMultiplier, 
            transform.position.y,transform.position.z);

        transform.position = newPos;
    }

}

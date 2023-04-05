using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundElementScroll : MonoBehaviour
{
    [Range(0.01f,1.0f)] public float scrollSpeed = 1.0f;
    [SerializeField] float startX = 0f;
    [SerializeField] float endX = 0f;
    void Update()
    {
        Vector3 positionThisFrame = new Vector3(transform.position.x + scrollSpeed * Time.deltaTime,transform.position.y,0);
        if (positionThisFrame.x > endX)
            transform.position = new Vector3(startX,transform.position.y,0);
        else
            transform.position = positionThisFrame;
    }

}

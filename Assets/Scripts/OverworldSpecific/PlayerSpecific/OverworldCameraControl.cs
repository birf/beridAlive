using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCameraControl : MonoBehaviour
{
    // mostly ripped straight from Joosung's demo project.
    [SerializeField] Transform target;
    [SerializeField]  float smoothSpeed;
    [SerializeField]  float maxX, minX, maxY, minY;

    private void Start()
    {
        if (!target)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        // MARKER traditional method
        //transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // MARKER smooth camera
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), smoothSpeed * Time.deltaTime);
        

        // MARKER limit range of camera
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX),
                                         Mathf.Clamp(transform.position.y, minY, maxY),
                                         transform.position.z);
    }
}

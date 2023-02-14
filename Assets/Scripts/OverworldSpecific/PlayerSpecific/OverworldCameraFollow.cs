using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 _offset = new Vector3(0,0,-10);
    [SerializeField] Transform _target;
    // Start is called before the first frame update

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _target.transform.position + _offset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //DamagePopup.Create(Vector3.zero, 300);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(9);
            //DamagePopup.Create(transform.position, 100);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hudTester : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] RuntimeAnimatorController runtime;

    PrimaryControls controls;


    void Start()
    {
        anim = GetComponent<Animator>();
        runtime = GetComponent<RuntimeAnimatorController>();
        controls = new PrimaryControls();
        controls.Enable();
    }

    void Update()
    {

        if (controls.Battle.Direction.triggered)
        {
            Vector2 v = controls.Battle.Direction.ReadValue<Vector2>();
            if (v.x == 1)
            {
                anim.Play("ready");
            }
            if (v.y == 1)
            {
                anim.Play("put_away");
            }
            if (v.x == -1)
            {
                anim.Play("low_health");
            }
            if (v.y == -1)
            {
                anim.Play("peril");
            }
        }
        if (controls.Battle.Primary.triggered)
            anim.Play("damage",-1,0);


    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ATKScript_Beri_Uppercut : ATKScript
{
    public BoxCollider2D puncher;

    bool _isCharging = false;
    bool _isPunching = false;
    float _timer;
    
    PrimaryControls controls;
    [SerializeField] LayerMask validLayers;
    Collider2D[] _hitBuffer = new Collider2D[3];

    void Awake()
    {
        controls = new PrimaryControls();
        controls.Enable();
    }
    void Update()
    {
        if (!_isPunching)
            FirstPhase();
        else
            SecondPhase();
    }
    void FirstPhase()
    {
        if (controls.Battle.Primary.triggered)
        {
            _isCharging = true;
        }
        if (_isCharging)
        {
            _timer += Time.deltaTime;
            if (controls.Battle.Primary.phase == InputActionPhase.Waiting)
            {
                if (_timer >= 0.25f)
                {
                    _timer = 0;
                    _isPunching = true;
                }
                else
                    OnFailure();
                
            }

        }
    }
    void SecondPhase()
    {
        _timer += Time.deltaTime;
        if (_timer >= 3 * Time.deltaTime)
            OnFailure();
        else
        {
            int hits = Physics2D.OverlapBoxNonAlloc(transform.position + (Vector3)puncher.offset,puncher.size,0f,_hitBuffer,validLayers);
            if (hits > 0)
            {
                for (int i = 0; i < _hitBuffer.Length; i++)
                {
                    if (_hitBuffer[i] != null)
                        if( _hitBuffer[i].gameObject.layer == 8 && _hitBuffer[i].gameObject == targetEnemy.gameObject)
                        {
                            targetEnemy.characterBattlePhysics.SetVelocity(parentMove.launchVelocity);
                            OnSuccess();
                            Destroy(gameObject);
                        }
                }
            }
        }
    }

    public override void BeginMove()
    {
        base.BeginMove();
    }
    public override void OnFailure()
    {
        Debug.Log("uh oh");
        controls.Disable();
        battleManager.PlayerAttackFailure();
        Destroy(gameObject);
    }
    public override void OnSuccess()
    {
        controls.Disable();
        battleManager.PlayerAttackSuccess();
        Destroy(gameObject);
    }
}

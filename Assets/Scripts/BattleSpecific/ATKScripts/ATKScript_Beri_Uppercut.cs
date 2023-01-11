using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AtkScript_Uppercut : ATKScript
{
    public BoxCollider2D puncher;

    bool _isCharging = false;
    bool _isPunching = false;
    float _timer;
    PrimaryControls controls;
    Collider2D[] _hitBuffer = new Collider2D[3];

    void Awake()
    {
        puncher.enabled = false;
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
            _isCharging = true;
        if (_isCharging)
        {
            _timer += Time.deltaTime;
            if (controls.Battle.Primary.triggered)
            {
                if (_timer >= 0.5f)
                {
                    _timer = 0;
                    _isPunching = true;
                }
                else
                {
                    OnFailure();
                }
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
            int hits = Physics2D.OverlapBoxNonAlloc(transform.position + (Vector3)puncher.offset,puncher.size,0f,_hitBuffer);
            if (hits > 0)
            {
                for (int i = 0; i < _hitBuffer.Length; i++)
                {
                    if (_hitBuffer[i] != null && _hitBuffer[i].tag == "Enemy" 
                        && _hitBuffer[i].name == targetEnemy.name)
                    {
                        targetEnemy.characterBattlePhysics.SetVelocity(new Vector2(0.15f,0.0f));
                        OnSuccess();
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    public override void OnFailure()
    {
        // controls.Disable();
        // battleManager.PlayerAttackFailure();
        // Destroy(gameObject);
    }
    public override void OnSuccess()
    {
        // controls.Disable();
        // battleManager.AdvanceActiveMove();
        // Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ATKScript_Beri_FryerBall : ATKScript
{
    /*
        ATKScript for Beri - FryerBall. Beri launches a fireball at the enemy, popping them up into the air and
        dealing damage. 
    */

    [SerializeField] GameObject _fireBall;
    [SerializeField] TextMeshPro chargeText;
    [SerializeField] int _charge = 0;
    [SerializeField] int _maxCharge = 110;
    [SerializeField] float _frameTimer = 0f;
    [SerializeField] float _secondsTimer = 0f;
    [SerializeField] float _timeLimit = 3f;
    [Range(1.0f,100.0f)][SerializeField] float _speed = 15.0f;

    int subPhase = 0;
    int localDamage = 0;
    PrimaryControls controls;

    void Awake()
    {
        controls = new PrimaryControls();
        controls.Enable();
    }
    
    void FixedUpdate()
    {
        switch (subPhase)
        {
            case (0) :
            {
                FirstPhase();
                break;
            }
            case (1) :
            {
                SecondPhase();
                break;
            }
        }
    }
    void Update()
    {
        if (controls.Battle.Primary.triggered)
        {
            _charge += 10;
        }
        chargeText.text = "" + _charge;
        if (_charge >= 100)
            chargeText.text = "100";
        if (_charge >= _maxCharge)
            _charge = _maxCharge;
    }
    void FirstPhase()
    {
        _frameTimer += Time.fixedDeltaTime;
        _secondsTimer += Time.fixedDeltaTime;
        if (_frameTimer > 2 * Time.fixedDeltaTime)
        {
            _charge -= 1;
            _frameTimer = 0;
            if (_charge < 0)
                _charge = 0;
        }
        if (_secondsTimer >= _timeLimit)
        {
            subPhase++;
            if (_charge == 0)
                OnFailure();
            else if (_charge > 0 && _charge < 50)
                localDamage = 1;
            else if (_charge > 50 && _charge < 99)
                localDamage = parentMove.damage;
            else
                localDamage = parentMove.damage + 1;
        }
    }
    void SecondPhase()
    {
        _fireBall.transform.position = Vector3.MoveTowards(_fireBall.transform.position,targetEnemy.transform.position, _speed * Time.fixedDeltaTime);
        if (Vector3.Distance(_fireBall.transform.position, targetEnemy.transform.position) < 0.01f)
        {
            targetEnemy.characterBattlePhysics.SetVelocity(parentMove.launchVelocity);
            targetEnemy.UpdateStat("Health",-localDamage);
            OnSuccess();
        }
    }
    public override void BeginMove()
    {
        base.BeginMove();
    }
    public override void OnSuccess()
    {
        controls.Disable();
        battleManager.PlayerAttackSuccess();
        base.OnSuccess();
        Destroy(gameObject);
    }
    public override void OnFailure()
    {
        controls.Disable();
        battleManager.PlayerAttackFailure();
        base.OnFailure();
        Destroy(gameObject);
    }

}

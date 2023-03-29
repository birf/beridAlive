using UnityEngine;
using TMPro;
using BeriUtils.Core;

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
    [Range(1.0f, 100.0f)] [SerializeField] float _speed = 15.0f;

    int subPhase = 0;
    int localDamage = 0;
    PrimaryControls controls;
    Timer timer;

    void Awake()
    {
        timer = new Timer(3.0f);
        timer.OnTimerEnd += CheckCharge;
        controls = new PrimaryControls();
        controls.Enable();
    }

    void FixedUpdate()
    {
        switch (subPhase)
        {
            case (0):
                {
                    FirstPhase();
                    break;
                }
            case (1):
                {
                    SecondPhase();
                    break;
                }
        }
    }
    protected override void Update()
    {
        chargeText.color = Color.black;
        chargeText.outlineColor = Color.black;

        if (controls.Battle.Primary.triggered)
        {
            _charge += 15;
            
        }
        chargeText.text = "" + _charge;
        if (_charge >= 100)
        {
            chargeText.text = "100";
            chargeText.outlineColor = Color.yellow;
            chargeText.color = Color.yellow;
        }
        if (_charge >= _maxCharge)
            _charge = _maxCharge;
    }
    void FirstPhase()
    {
        timer.Tick(Time.fixedDeltaTime);
        _charge -= 1;
        if (_charge < 0)
            _charge = 0;
    }
    void CheckCharge()
    {
        subPhase++;
        if (_charge == 0)
            OnFailure();
        else if (_charge > 0 && _charge < 50)
            localDamage = parentMove.damage;
        else if (_charge > 50 && _charge < 99)
            localDamage = parentMove.damage + 1;
        else
            localDamage = parentMove.damage + 2;
    }
    void SecondPhase()
    {
        _fireBall.transform.position = Vector3.MoveTowards(_fireBall.transform.position, targetEnemy.transform.position, _speed * Time.fixedDeltaTime);
        if (Vector3.Distance(_fireBall.transform.position, targetEnemy.transform.position) < 0.01f)
        {
            OnSuccess();
        }
    }
    public override void BeginMove()
    {
        base.BeginMove();
        parentMove.damage = caster.characterData.curATK + 1;
    }
    public override void OnSuccess()
    {
        base.OnSuccess(localDamage);
        controls.Disable();
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

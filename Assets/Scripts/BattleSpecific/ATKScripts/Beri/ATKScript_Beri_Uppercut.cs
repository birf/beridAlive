using UnityEngine;
using UnityEngine.InputSystem;
using BeriUtils.Core;
public class ATKScript_Beri_Uppercut : ATKScript
{
    public BoxCollider2D puncher;

    bool _isCharging = false;
    bool _isPunching = false;
    float _timer;
    
    PrimaryControls controls;
    Timer timer = new Timer(2);
    [SerializeField] LayerMask validLayers;
    Collider2D[] _hitBuffer = new Collider2D[3];

    void Awake()
    {
        timer.OnTimerEnd += OnFailure;
        controls = new PrimaryControls();
        controls.Enable();
    }
    protected override void Update()
    {
        timer.Tick(Time.deltaTime);
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
            // check to see if enemy is in hitbox this frame.
            int hits = 0;
            hits = Physics2D.OverlapBoxNonAlloc(puncher.transform.position + (Vector3)puncher.offset, puncher.size,0f,_hitBuffer,validLayers);
            if (hits > 0)
            {
                for (int i = 0; i < _hitBuffer.Length; i++)
                {
                    if (_hitBuffer[i] != null)
                        if( _hitBuffer[i].gameObject.layer == 8 && _hitBuffer[i].gameObject == targetEnemy.gameObject)
                        {
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
        controls.Disable();
        battleManager.PlayerAttackFailure();
        base.OnFailure();
        Destroy(gameObject);
    }
    public override void OnSuccess()
    {
        targetEnemy.characterBattlePhysics.SetVelocity(parentMove.mainLaunchVelocity);
        targetEnemy.characterData.UpdateStat("Health", -parentMove.damage);

        controls.Disable();
        battleManager.AttackSuccess();
        base.OnSuccess();
        Destroy(gameObject);
    }
}

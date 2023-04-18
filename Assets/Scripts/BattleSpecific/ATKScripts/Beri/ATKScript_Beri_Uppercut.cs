using UnityEngine;
using UnityEngine.InputSystem;
using BeriUtils.Core;
public class ATKScript_Beri_Uppercut : ATKScript
{
    public BoxCollider2D puncher;
    bool _isCharging = false;
    bool _isPunching = false;
    bool _isInPosition = false;
    float _chargeTimer;
    
    PrimaryControls controls;
    Timer timer = new Timer(2);
    [SerializeField] LayerMask validLayers;
    Collider2D[] _hitBuffer = new Collider2D[3];

    void Awake()
    {
        timer.OnTimerEnd += OnFailure;
        controls = new PrimaryControls();
        controls.Enable();
        BeginMove();
        battleManager.currentActiveCharacter.characterBattlePhysics.characterPhysicsState
            = BattlePhysicsInteraction.CharacterPhysicsState.DEFAULT;
    }
    protected override void Update()
    {
        caster.GetComponent<Animator>().runtimeAnimatorController = parentMove.moveSpecificAnimations;
        if (!_isInPosition)
            WalkingPhase();
        if (!_isPunching && _isInPosition)
            ChargingPhase();
        else if (_isPunching && _isInPosition)
            PunchingPhase();

        if (_isInPosition)
            timer.Tick(Time.deltaTime);
    }

    void WalkingPhase()
    {
        PlayAnimation("beri_battle_idle");

        Vector3 targetPos = battleManager.currentActiveCharacter.transform.position;
        if (previousMoveType != MoveType.GRAB && previousMoveType != MoveType.PUNCH)
        {
            targetPos = targetEnemy.transform.position + Vector3.left * 2.5f;
            battleManager.currentActiveCharacter.characterBattlePhysics.MoveToPosition(targetPos);
            gameObject.transform.position = battleManager.currentActiveCharacter.transform.position;
        }
        if (battleManager.currentActiveCharacter.transform.position == targetPos)
            _isInPosition = true;
    }
    void ChargingPhase()
    {
        battleManager.currentActiveCharacter.GetComponent<SpriteRenderer>().color = Color.white;
        bool chargeReady = (_chargeTimer >= 0.25f);
        
        // if her charge is ready, flash to let player know.
        if (chargeReady)
        {
            if (_chargeTimer > 0.25f)
                caster.GetComponent<SpriteRenderer>().color = Color.grey;
            else if (_chargeTimer >= 0.5f)
            {
                caster.GetComponent<SpriteRenderer>().color = Color.white;
                _chargeTimer = 0;
            }
        }
        // commence charging when player hits enter.
        if (controls.Battle.Primary.triggered)
            _isCharging = true;

        if (_isCharging)
        {
            PlayAnimation("beri_uppercut_windup");
            _chargeTimer += Time.deltaTime;
            // only let go of the punch when button is releasted.
            if (controls.Battle.Primary.phase == InputActionPhase.Waiting)
            {
                if (chargeReady)
                {
                    _chargeTimer = 0;
                    _isPunching = true;
                }
                else
                    OnFailure();
                
            }
        }
        
    }
    void PunchingPhase()
    {
        PlayAnimation("beri_uppercut_punch");
        caster.GetComponent<SpriteRenderer>().color = Color.white;
        _chargeTimer += Time.deltaTime;
        if (_chargeTimer >= 3 * Time.deltaTime)
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
        cooldownTimer.OnTimerEnd += SetRecovery;
        base.OnFailure();
        controls.Disable();
        battleManager.PlayerAttackFailure();
        Destroy(gameObject);
    }
    public override void OnSuccess()
    {
        cooldownTimer.OnTimerEnd += SetRecovery;
        caster.characterBattlePhysics.Jump(new Vector2(0,2.5f));
        caster.characterBattlePhysics.shouldImmediatelyRecover = false;
        base.OnSuccess();
        controls.Disable();
        Destroy(gameObject);
    }
    void SetRecovery()
    {
        caster.characterBattlePhysics.characterPhysicsState = BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY;
    }
}

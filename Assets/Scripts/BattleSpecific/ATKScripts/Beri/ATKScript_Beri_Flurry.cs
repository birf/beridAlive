using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;
public class ATKScript_Beri_Flurry : ATKScript
{
    /*
    */

    BattlePhysicsInteraction _characterPhysics;

    [SerializeField] LayerMask validLayers;
    [SerializeField] BoxCollider2D _firstHitbox;
    [SerializeField] BoxCollider2D _airborneHitBox;
    PrimaryControls controls;
    Collider2D[] _hitBuffer = new Collider2D[3];
    Timer _timeoutTimer = new Timer(0.25f);

    int subphase = 0;


    bool _isInPosition = false;
    bool _isJumping = false;
    void Awake()
    {
        _characterPhysics = caster.characterBattlePhysics;
        transform.parent = caster.transform;

        _timeoutTimer.OnTimerEnd += OnFailure;
        controls = new PrimaryControls();
        controls.Enable();
        BeginMove();
        
        battleManager.currentActiveCharacter.characterBattlePhysics.characterPhysicsState
            = BattlePhysicsInteraction.CharacterPhysicsState.DEFAULT;
        
    }
    protected override void Update()
    {
        _characterPhysics = caster.characterBattlePhysics;
        _isJumping = _characterPhysics.isJumping;

        // if we aren't nearby the enemy or the previous move doesn't bring the enemy over to us, walk to them.
        if (!_isInPosition)
            WalkingPhase();
        if (_isJumping && _isInPosition && subphase < 3)
            JumpingPhase();
        if (subphase >= 3 && subphase < 6)
            AirbornePhase();

        // by this point, all punches would have connected and the move is complete.
        if (subphase >= 6)
            OnSuccess();
        
        // if at any point we make it back to the ground before the move is complete, end the move due to failure.
        if (subphase > 0 && _characterPhysics.isGrounded == true)
            OnFailure(); 
        
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
        {
            _isInPosition = true;
            _characterPhysics.Jump();
            subphase++;
        }
    }
    void JumpingPhase()
    {
        bool correctInput = (controls.Battle.Direction.ReadValue<Vector2>().y == -1.0
                          && controls.Battle.Primary.triggered);
        
        if (correctInput)
            subphase++;
        if (subphase == 2)
        {
            _timeoutTimer.Tick(Time.deltaTime);

            // check to see if enemy is in hitbox this frame.
            int hits = 0;
            hits = Physics2D.OverlapBoxNonAlloc(_firstHitbox.transform.position + (Vector3)_firstHitbox.offset, 
                                                _firstHitbox.size,0f,_hitBuffer,validLayers);
            if (hits > 0)
            {
                for (int i = 0; i < _hitBuffer.Length; i++)
                {
                    if (_hitBuffer[i] != null)
                        if( _hitBuffer[i].gameObject.layer == 8 && _hitBuffer[i].gameObject == targetEnemy.gameObject)
                        {
                            DamagePopup.Create(battleManager.currentTargetCharacter.transform.position, 
                            parentMove.damage - targetEnemy.characterData.curDEF);

                            targetEnemy.characterBattlePhysics.HitTarget(new Vector2(0.5f,-5f),1);
                            _characterPhysics.LaunchTarget(new Vector2(0.5f,3.0f));
                            subphase++;
                            _timeoutTimer = new Timer(0.25f);
                            break;
                        }
                }
            }
        }
    }
    void AirbornePhase()
    {
        _timeoutTimer.Tick(Time.deltaTime);
        bool correctInput = (controls.Battle.Primary.triggered && _timeoutTimer.RemaingSeconds <= 0);

        if (correctInput)
        {
            int hits = 0;
            hits = Physics2D.OverlapBoxNonAlloc(_airborneHitBox.transform.position + (Vector3)_airborneHitBox.offset, 
                                                _airborneHitBox.size,0f,_hitBuffer,validLayers);
            if (hits > 0)
            {
                for (int i = 0; i < _hitBuffer.Length; i++)
                {
                    if (_hitBuffer[i] != null)
                        if( _hitBuffer[i].gameObject.layer == 8 && _hitBuffer[i].gameObject == targetEnemy.gameObject)
                        {
                            
    
                            if (subphase < 5)
                            {
                                targetEnemy.characterBattlePhysics.HitTarget(new Vector2(0.5f,3.0f),1);
                                DamagePopup.Create(battleManager.currentTargetCharacter.transform.position, 
                                parentMove.damage - targetEnemy.characterData.curDEF);
                            }
                            Debug.Log(subphase);
                            _characterPhysics.LaunchTarget(new Vector2(0.5f,2.5f));
                            _timeoutTimer = new Timer(0.25f);
                            subphase++;
                            break;
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
        caster.characterBattlePhysics.shouldImmediatelyRecover = true;
        base.OnSuccess();
        controls.Disable();
        Destroy(gameObject);
    }
    void SetRecovery()
    {
        if (caster.characterBattlePhysics.isGrounded)
            caster.characterBattlePhysics.characterPhysicsState = BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY;
    }
}

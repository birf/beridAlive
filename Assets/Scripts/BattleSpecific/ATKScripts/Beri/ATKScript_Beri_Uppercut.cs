using UnityEngine;
using UnityEngine.InputSystem;
using BeriUtils.Core;
public class ATKScript_Beri_Uppercut : ATKScript
{
    public BoxCollider2D puncher;

    bool _isCharging = false;
    bool _isPunching = false;
    bool _isInPosition = false;
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
        BeginMove();
        battleManager.currentActiveCharacter.characterBattlePhysics.characterPhysicsState
            = BattlePhysicsInteraction.CharacterPhysicsState.DEFAULT;
    }
    protected override void Update()
    {
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
        battleManager.currentActiveCharacter.GetComponent<SpriteRenderer>().color = Color.black;
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
    void PunchingPhase()
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
        battleManager.currentActiveCharacter.characterBattlePhysics.characterPhysicsState 
                                = BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY;
        controls.Disable();
        battleManager.PlayerAttackFailure();
        base.OnFailure();
        Destroy(gameObject);
    }
    public override void OnSuccess()
    {
        battleManager.currentActiveCharacter.characterBattlePhysics.characterPhysicsState 
                                = BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY;
        base.OnSuccess();
        controls.Disable();
        Destroy(gameObject);
    }
}

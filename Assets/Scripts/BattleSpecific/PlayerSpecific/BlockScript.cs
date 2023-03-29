using UnityEngine;
using BeriUtils.Core;


[RequireComponent(typeof(CharacterGameBattleEntity))]
public class BlockScript : MonoBehaviour
{

    [SerializeField] CharacterGameBattleEntity _characterBody;
    PrimaryControls controls;
    Timer _blockTimer;
    Timer _parryTimer;
    Timer _cooldownTimer;

    [SerializeField] float _blockDuration; // how long the block state lasts
    [SerializeField] float _parryDuration; // how long the parry duration lasts.
    [SerializeField] float _cooldownDuration; // how long the cooldown lasts.
    [SerializeField] LayerMask _validLayers;

    enum BlockPhase 
    {
        NONE,
        PARRY,
        BLOCK,
        COOLDOWN
    }
    [SerializeField] BlockPhase _currentBlockPhase;
    SpriteRenderer _test;
    void OnEnable()
    {
        controls = new PrimaryControls();
        controls.Enable();
        _characterBody = GetComponent<CharacterGameBattleEntity>();

        _blockTimer = new Timer(_blockDuration);
        _parryTimer = new Timer(_parryDuration);
        _cooldownTimer = new Timer(_cooldownDuration);

        _blockTimer.OnTimerEnd += ChangeState;
        _parryTimer.OnTimerEnd += ChangeState;
        _cooldownTimer.OnTimerEnd += ChangeState;

        _test = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // only ever have this script function if the current state is "ENEMYTURN".
        if (BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.ENEMYTURN)
        {
            if (controls.Battle.Primary.triggered && _currentBlockPhase == BlockPhase.NONE)
                _currentBlockPhase = BlockPhase.PARRY;
            switch (_currentBlockPhase)
            {
                case (BlockPhase.PARRY) :
                {
                    ParryingPhase();
                    break;
                }
                case (BlockPhase.BLOCK) :
                {
                    BlockingPhase();
                    break;
                }
                case(BlockPhase.COOLDOWN) :
                {
                    CooldownPhase();
                    break;
                }
                default :
                    _test.color = Color.white;
                    break;
            }
        }
        else
            CompleteReset();
        if (CheckCollisions())
            CompleteReset();
    }
    
    void ParryingPhase()
    {
        // _characterBody.characterData.curDEF = 99;
        _parryTimer.Tick(Time.deltaTime);
        _test.color = Color.yellow; 
    }
    void BlockingPhase()
    {
        // _characterBody.characterData.curDEF = _characterBody.characterData.baseDEF + 1;
        _blockTimer.Tick(Time.deltaTime);
        _test.color = Color.green;
    }
    void CooldownPhase()
    {
        // _characterBody.characterData.curDEF = _characterBody.characterData.baseDEF;
        _cooldownTimer.Tick(Time.deltaTime);
        _test.color = Color.blue;
    }
    void ChangeState()
    {
        _currentBlockPhase += 1;
        if ((int)_currentBlockPhase > 3)
            _currentBlockPhase = 0;
        
        ResetTimers();
    }
    void ResetTimers()
    {
        _blockTimer.SetTimer(_blockDuration);
        _parryTimer.SetTimer(_parryDuration);
        _cooldownTimer.SetTimer(_cooldownDuration);

        _test.color = Color.white;

    }
    void CompleteReset()
    {
        ResetTimers();
        _currentBlockPhase = BlockPhase.NONE;
    }

    public bool CheckCollisions()
    {
        Collider2D[] buffer = new Collider2D[1];
        int hits = Physics2D.OverlapBoxNonAlloc(transform.position,_characterBody.characterScriptable.battleHitBoxSize,0f,buffer,_validLayers);
        if (hits > 0)
            return true;
        return false;
    }
    public bool CheckCollisions(out int damageReduction)
    {
        Collider2D[] buffer = new Collider2D[1];
        int hits = Physics2D.OverlapBoxNonAlloc(transform.position,_characterBody.characterScriptable.battleHitBoxSize,0f,buffer,_validLayers);
        if (hits > 0)
        {
            switch (_currentBlockPhase)
            {
                case BlockPhase.PARRY :
                    damageReduction = 999;
                    return true;
                case BlockPhase.BLOCK :
                    damageReduction = 1;
                    return true;
                case BlockPhase.NONE :
                case BlockPhase.COOLDOWN :
                    damageReduction = 0;
                    return true;
            }
        }
        damageReduction = 0;
        return false;
    }

}

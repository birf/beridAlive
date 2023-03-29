using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKScript : MonoBehaviour
{
    /*
        Script from which all battle moves' attack script field should be placed. These scripts are responsible for instantiating the
        hitboxes required for the move as well as assign animations to the caster, among other features.
    */

    public BattleMove parentMove;
    public CharacterGameBattleEntity targetEnemy;
    public CharacterGameBattleEntity caster;
    public BattleManager battleManager;
    public MoveType previousMoveType;


    protected virtual void Update()
    {
    }

    protected void StateCheck()
    {
        // tbh idk why this is here.
        if (BattleManager.CurrentBattleManagerState != BattleManager.BattleManagerState.PLAYERATTACK &&
            targetEnemy.characterData.CharType != CharacterBase.CharacterType.PLAYER)
            OnFailure();
    }
    public virtual void BeginMove()
    {
        bool flag = false;
        if (battleManager == null)
        {
            Debug.Log("Error : battleManager never initialized! On : " + gameObject.name);
            flag = true;
        } 
        if (targetEnemy == null)
        {
            Debug.Log("Error : targetEnemy never initialized! On : " + gameObject.name);
            flag = true;
        }
        if (parentMove == null)
        {
            Debug.Log("Error : parentMove never initialized! On : " + gameObject.name);
            flag = true;
        }
        if (flag)
            Destroy(gameObject);

        battleManager.currentTargetCharacter = targetEnemy;
        caster = battleManager.currentActiveCharacter;

        
        if (battleManager.battleManagerMoveQueue.Count != 0 && battleManager.battleManagerMoveQueueIndex != 0)
            previousMoveType = battleManager.battleManagerMoveQueue[battleManager.battleManagerMoveQueueIndex-1].moveType;
        else
            previousMoveType = MoveType.NONE;
    }  
    
    ///<summary>
    /// When an attack is successful, the attack script will alert the current battle manager and 
    /// deal the appropriate damage and knockback to the entity. When creating a battle script, always have 
    /// this called somewhere within it. Be careful not to reuse functions within!
    ///</summary>

    public virtual void OnSuccess()
    {
        DamagePopup.Create(battleManager.currentTargetCharacter.transform.position, 
                            parentMove.damage - targetEnemy.characterData.curDEF);

        targetEnemy.characterBattlePhysics.HitTarget(parentMove.mainLaunchVelocity, parentMove.damage);
        battleManager.AttackSuccess();
        previousMoveType = MoveType.NONE;

    }
    public virtual void OnSuccess(int damageOverride)
    {
        DamagePopup.Create(battleManager.currentTargetCharacter.transform.position, 
                            damageOverride - targetEnemy.characterData.curDEF);

        targetEnemy.characterBattlePhysics.HitTarget(parentMove.mainLaunchVelocity,
                                                     damageOverride - targetEnemy.characterData.curDEF);
        battleManager.AttackSuccess();
        previousMoveType = MoveType.NONE;

    }
    public virtual void OnFailure()
    {
        BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ANALYSIS;
        previousMoveType = MoveType.NONE;
    }
}

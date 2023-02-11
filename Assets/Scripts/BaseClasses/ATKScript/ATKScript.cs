using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKScript : MonoBehaviour
{
    /*
        Script from which all battle moves' attack script field should be placed. These scripts are responsible for instantiating the
        hitboxes required for the move as well as assign animations to the caster, among other features.

        
        IMPORTANT : 
        When making a new ATKScript, make sure that each Update method (if used) is prefaced by :
            "protected override void"
        This is done to inherit any base functionality from this script's update loop. For now it simply checks if at any point the 
        player times out (i.e. : their target enemy is in recovery ) their move will fail.

    */

    public BattleMove parentMove;
    public CharacterGameBattleEntity targetEnemy;
    public BattleManager battleManager;

    protected virtual void Update()
    {
        StateCheck();
    }
    protected virtual void LateUpdate()
    {}
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
        
    }  
    public virtual void OnSuccess()
    {
        battleManager.AttackSuccess();
    }
    public virtual void OnFailure()
    {
        BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ANALYSIS;
    }
}

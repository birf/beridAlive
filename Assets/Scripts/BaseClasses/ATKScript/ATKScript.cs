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
    public CharacterGameEntity targetEnemy;
    public BattleManager battleManager;

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
    }  
    public virtual void OnSuccess(){}
    public virtual void OnFailure()
    {
        BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ENEMYTURN;
    }
}

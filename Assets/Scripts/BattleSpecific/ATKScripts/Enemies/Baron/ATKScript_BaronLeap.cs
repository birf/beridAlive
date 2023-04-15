using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKScript_BaronLeap : ATKScript
{
    /*
        ATKScript for the Baron enemy. He jumps onto the target in a big ol' lunge.
    */
    int subphase = 0;


    void Awake()
    {
        base.BeginMove();
    }
    protected override void Update() 
    {
        transform.position = caster.transform.position;
        
        if (subphase <= 1)
            JumpPhase();
        else
            LeapToTarget();
    }
    void JumpPhase()
    {
        if (subphase == 0)
        {
            caster.characterBattlePhysics.Jump();
            subphase++;
        }
        else
        {
            if (caster.characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.DEFAULT)
                subphase++;
        }
    }
    void LeapToTarget()
    {
        battleManager.currentActiveCharacter.characterBattlePhysics.MoveToPosition(targetEnemy.transform.position,20.0f);
        if (targetEnemy.GetComponent<BlockScript>().CheckCollisions(out int damageReduction))
        {
            battleManager.currentActiveCharacter.characterBattlePhysics.LaunchTarget(new Vector2(-1,2));
            base.OnSuccess(parentMove.damage - damageReduction);
            Destroy(gameObject);
        }
    }   
}

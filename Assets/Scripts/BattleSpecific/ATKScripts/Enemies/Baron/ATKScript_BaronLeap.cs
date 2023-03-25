using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKScript_BaronLeap : ATKScript
{
    /*
        ATKScript for the Baron enemy. He jumps onto the target in a big ol' lunge.
    */
    int subphase = 0;

    protected override void Update() 
    {
        transform.position = battleManager.currentActiveCharacter.transform.position;
        if (subphase == 0)
            LeapToTarget();
        if (subphase == 1)
            OnSuccess();
    }
    void LeapToTarget()
    {
        battleManager.currentActiveCharacter.characterBattlePhysics.MoveToPosition(targetEnemy.transform.position,20.0f);
        if (transform.position == targetEnemy.transform.position)
            subphase++;
    }    
    public override void OnSuccess()
    {
        battleManager.currentActiveCharacter.characterBattlePhysics.LaunchTarget(new Vector2(-1,2));
        base.OnSuccess();
        Destroy(gameObject);
    }

}

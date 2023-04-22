using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterGameBattleEntity))]
public class BasicEnemyAI : MonoBehaviour
{
    public CharacterGameBattleEntity enemyEntity;
    public bool canExecute = true;
    void Awake()
    {
        enemyEntity = GetComponent<CharacterGameBattleEntity>();
        // ^^ why doesn't this fucking work ^^ //
    }

    public void Execute()
    {
        if (canExecute)
            DecideNextMove();
    }

    void DecideNextMove()
    {
        // for now, just do this;
        List<BattleMove> l = new List<BattleMove>();
        
        enemyEntity.characterScriptable.characterMoves[0].mainMoveGameObject.GetComponent<ATKScript>().caster = enemyEntity;

        // ^ this is the single dumbest fucking line i've ever written in my life ^ //

        int randomMove = (int)Mathf.Floor(Random.Range(0f,enemyEntity.characterScriptable.characterMoves.Count));
        l.Add(enemyEntity.characterScriptable.characterMoves[randomMove]);

        enemyEntity.entityBattleManager.FeedMoveQueue(l, enemyEntity.entityBattleManager.playerCharacters[0]);
        enemyEntity.entityBattleManager.StartAttack();
        enemyEntity.characterAnimator.Play("attack0" + (randomMove + 1));

        Debug.Log("attack0" + (randomMove + 1));
        
        canExecute = false;
    }

}

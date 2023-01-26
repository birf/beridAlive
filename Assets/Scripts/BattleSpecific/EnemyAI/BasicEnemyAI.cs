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
    }

    public void Execute()
    {
        Debug.Log("in execute function for " + gameObject.name);
        if (canExecute)
            DecideNextMove();
    }

    void DecideNextMove()
    {
        // for now, just do this;
        List<BattleMove> l = new List<BattleMove>();
        l.Add(enemyEntity.characterScriptable.characterMoves[0]);
        enemyEntity.entityBattleManager.FeedMoveQueue(l,enemyEntity.entityBattleManager.playerCharacters[0]);
        enemyEntity.entityBattleManager.StartAttack();
        canExecute = false;
    }

}

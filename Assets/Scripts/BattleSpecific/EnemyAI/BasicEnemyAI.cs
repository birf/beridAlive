using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterGameEntity))]
public class BasicEnemyAI : MonoBehaviour
{
    public CharacterGameEntity enemyEntity;
    void Awake()
    {
        enemyEntity = GetComponent<CharacterGameEntity>();
    }
    void Update()
    {
        if (BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.ENEMYTURN)
            Debug.Log("woohoo");
    }
}

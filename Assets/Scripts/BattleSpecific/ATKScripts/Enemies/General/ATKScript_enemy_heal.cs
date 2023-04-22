using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

public class ATKScript_enemy_heal : ATKScript
{
    Timer countdown = new Timer(2);
    [SerializeField] GameObject visualEffect;

    void Awake()
    {
        base.BeginMove();
        visualEffect.GetComponent<SpriteRenderer>().color = Color.green;
        countdown.OnTimerEnd += OnSuccess;
        FindEnemyWithLowestHealth();
    }
    protected override void Update()
    {
        visualEffect.transform.position = targetEnemy.transform.position;
        countdown.Tick(Time.deltaTime);
    }
    public override void OnSuccess()
    {
        base.OnSuccess(0);
        targetEnemy.characterData.AddToStat(CharacterStat.HP,3,false);
        targetEnemy.characterBattlePhysics.Jump();
        Destroy(gameObject);
    }
    void FindEnemyWithLowestHealth()
    {
        List<CharacterGameBattleEntity> enemies = battleManager.enemyCharacters;
        int lowest = 999;
        CharacterGameBattleEntity leastHealthy = enemies[0];

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].characterData.curHP < lowest)
            {
                lowest = enemies[i].characterData.curHP;
                leastHealthy = enemies[i];
            }
        }
        targetEnemy = leastHealthy;
        battleManager.currentTargetCharacter = battleManager.playerCharacters[0];
    }
}

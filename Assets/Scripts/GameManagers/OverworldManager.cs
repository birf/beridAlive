using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : GameManager
{
    public CharacterGameOverworldEntity currentEnemyEncounter;

    [SerializeField] GameObject @object;


    [SerializeField] GameObject entities;

    [SerializeField] List<Vector2> battlePositions;
    void Start()
    {
        CentralManager.SetStateManager(this);
        CentralManager.CurrentContext = CentralManager.Context.OVERWORLD;
        OverworldManagerSetup();
    }
    void OnEnable()
    {
        CentralManager.CurrentContext = CentralManager.Context.OVERWORLD;
        CentralManager.SetStateManager(this);
    }
    void OnDisable()
    {
        CentralManager.CurrentContext = CentralManager.Context.BATTLE;
        CentralManager.SetStateManager((BattleManager)ChildObjects[0].GetComponent<BattleManager>());
    }

    GameObject CreateEnemy(CharacterScriptable characterScriptable)
    {
        GameObject enemy = Instantiate(@object);
        CharacterGameBattleEntity entity = enemy.GetComponent<CharacterGameBattleEntity>();
        entity.characterScriptable = characterScriptable;
        entity.characterAnimations = characterScriptable.charAnimations;

        enemy.transform.parent = entities.transform;


        return enemy;

    }

    public void BattleStart()
    {
        BattleManager bm = ChildObjects[0].GetComponent<BattleManager>();
        bm.gameObject.SetActive(true);


        for (int i = 0; i < currentEnemyEncounter.partnerCharacters.Count; i++)
        {
            GameObject enemy = CreateEnemy(currentEnemyEncounter.partnerCharacters[i]);
            enemy.transform.localPosition = battlePositions[i];
            enemy.GetComponent<BattlePhysicsInteraction>().startPosition = battlePositions[i];
        }


        gameObject.SetActive(false);
    }
    void OverworldManagerSetup()
    {
        CharacterGameOverworldEntity[] characterEntities = FindObjectsOfType<CharacterGameOverworldEntity>();
        for (int i = 0; i < characterEntities.Length; i++)
        {
            Characters.Add(characterEntities[i].characterData);
        }
    }

}

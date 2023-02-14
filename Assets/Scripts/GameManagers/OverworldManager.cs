using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : GameManager
{
    public CharacterGameOverworldEntity currentEnemyEncounter;
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

    public void BattleStart()
    {
        BattleManager bm = ChildObjects[0].GetComponent<BattleManager>();
        bm.gameObject.SetActive(true);
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

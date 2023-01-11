using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : GameManager
{
    void OnEnable()
    {
        CharacterGameObjects = new List<CharacterGameEntity>(FindObjectsOfType<CharacterGameEntity>());
        CentralManager.SetStateManager(this);
        CentralManager.CurrentContext = CentralManager.Context.BATTLE; 
    }
    public void Tester()
    {
        Debug.Log("Battle Manager woohoo");
    }
}

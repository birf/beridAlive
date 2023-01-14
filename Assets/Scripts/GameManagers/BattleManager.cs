using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : GameManager
{
    public List<ItemData> playerItems = new List<ItemData>();
    public List<BattleMove> playerMoves = new List<BattleMove>();
    void Awake()
    {
        CentralManager.SetStateManager(this);
        CharacterGameObjects = new List<CharacterGameEntity>(FindObjectsOfType<CharacterGameEntity>());
        CentralManager.CurrentContext = CentralManager.Context.BATTLE; 
        
        for (int i = 0; i < CharacterGameObjects.Count; i++)
        {
            if (CharacterGameObjects[i].characterData.CharType == CharacterBase.CharacterType.PLAYER)
            {
                playerItems = new List<ItemData>(CharacterGameObjects[i].characterScriptable.characterItems);
                playerMoves = new List<BattleMove>(CharacterGameObjects[i].characterScriptable.characterMoves);
                break;
            }
        }
    }
}

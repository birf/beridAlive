using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : GameManager
{
    public List<ItemData> playerItems = new List<ItemData>();
    public List<BattleMove> playerMoves = new List<BattleMove>();

    public List<CharacterGameEntity> playerCharacters = new List<CharacterGameEntity>();
    public List<CharacterGameEntity> enemyCharacters = new List<CharacterGameEntity>();

    public CharacterGameEntity currentActiveCharacter;


    void Awake()
    {
        BattleManagerSetup();
        CentralManager.CurrentContext = CentralManager.Context.BATTLE; 
    }
    void BattleManagerSetup()
    {
        CentralManager.SetStateManager(this);
        CharacterGameObjects = new List<CharacterGameEntity>(FindObjectsOfType<CharacterGameEntity>());
        
        for (int i = 0; i < CharacterGameObjects.Count; i++)
        {
            // if the character type is the player, setup their moves and items. 
            CharacterGameObjects[i].characterSelectable.isDestroyable = false;
            CharacterGameObjects[i].characterSelectable.canBeDisabled = false;

            if (CharacterGameObjects[i].characterData.CharType == CharacterBase.CharacterType.PLAYER) 
            {
                playerItems = new List<ItemData>(CharacterGameObjects[i].characterScriptable.characterItems);
                playerMoves = new List<BattleMove>(CharacterGameObjects[i].characterScriptable.characterMoves);
                playerCharacters.Add(CharacterGameObjects[i]);
                CharacterGameObjects[i].characterSelectable.cyclableElements += 1;
            }
            if (CharacterGameObjects[i].characterData.CharType == CharacterBase.CharacterType.ENEMY)
            {
                enemyCharacters.Add(CharacterGameObjects[i]);
                CharacterGameObjects[i].characterSelectable.cyclableElements += 1;
            }
        }
        currentActiveCharacter = playerCharacters[0]; // <-- testing 
    }
}

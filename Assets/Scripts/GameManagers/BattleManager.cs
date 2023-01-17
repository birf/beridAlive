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

    List<BattleMove> _playerMoveQueueBM = new List<BattleMove>();

    public enum BattleManagerState
    {
        DEFAULT,
        PLAYERTURN,
        PLAYERATTACK,
        ENEMYTURN
    }
    public BattleManagerState currentBattleManagerState;

    void Awake()
    {
        currentBattleManagerState = BattleManagerState.DEFAULT;
        BattleManagerSetup();
        CentralManager.CurrentContext = CentralManager.Context.BATTLE; 
    }
    void Update() 
    {
        switch (currentBattleManagerState)
        {
            case (BattleManagerState.DEFAULT) :
            {
                break;
            }
            case (BattleManagerState.PLAYERTURN) :
            {
                break;
            }
            case (BattleManagerState.PLAYERATTACK) :
            {
                Debug.Log("woohoo");
                break;
            }
        }   
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

    // Feed the move queue into the battle manager to start attacking.
    public void FeedPlayerMoveQueue(List<BattleMove> playerMoves, CharacterGameEntity targetEnemy)
    {
        _playerMoveQueueBM = playerMoves;
        for (int i = 0; i < _playerMoveQueueBM.Count; i++)
        {
            _playerMoveQueueBM[i].mainMoveGameObject.GetComponent<ATKScript>().targetEnemy = targetEnemy;
            _playerMoveQueueBM[i].mainMoveGameObject.GetComponent<ATKScript>().parentMove = _playerMoveQueueBM[i] ;
            _playerMoveQueueBM[i].mainMoveGameObject.GetComponent<ATKScript>().battleManager = this;
        }
        Debug.Log("woohoo");
    }
}

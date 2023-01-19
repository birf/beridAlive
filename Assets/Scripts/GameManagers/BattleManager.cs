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

    [SerializeField] List<BattleMove> _playerMoveQueueBM = new List<BattleMove>();

    public enum BattleManagerState
    {
        DEFAULT,
        PLAYERTURN,
        PLAYERATTACK,
        ENEMYTURN
    }
    public static BattleManagerState CurrentBattleManagerState;

    void Start()
    {
        CurrentBattleManagerState = BattleManagerState.DEFAULT;
        BattleManagerSetup();

        CentralManager.CurrentContext = CentralManager.Context.BATTLE; 
    }
    void Update() 
    {
        switch (CurrentBattleManagerState)
        {
            case (BattleManagerState.DEFAULT) :
            {
                Debug.Log("Default");
                break;
            }
            case (BattleManagerState.PLAYERTURN) :
            {
                Debug.Log("PlayerTurn");
                break;
            }
            case (BattleManagerState.PLAYERATTACK) :
            {
                Debug.Log("PlayerAttack");
                break;
            }
            case (BattleManagerState.ENEMYTURN) :
            {
                Debug.Log("EnemyTurn");
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
        _playerMoveQueueBM = new List<BattleMove>(playerMoves);
        for (int i = 0; i < _playerMoveQueueBM.Count; i++)
        {
            if (_playerMoveQueueBM[i].mainMoveGameObject == null)
            {
                Debug.Log("ERROR : " + _playerMoveQueueBM[i].moveName + " has no hitbox! Aborting.");
                break;
            }
            _playerMoveQueueBM[i].SetupMainMoveGameObject(targetEnemy,_playerMoveQueueBM[i],this);
            _playerMoveQueueBM[i].mainMoveGameObject.transform.position = currentActiveCharacter.transform.position;
        }
    }
    // start attacking.
    public void StartAttack()
    {
        _playerMoveQueueBM[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
        Instantiate(_playerMoveQueueBM[0].mainMoveGameObject,currentActiveCharacter.transform.position,Quaternion.identity);

        _playerMoveQueueBM.RemoveAt(0);
    }
    public void PlayerAttackSuccess()
    {
        if (_playerMoveQueueBM.Count == 0)
            Debug.Log("Success!");
        else
        {
            _playerMoveQueueBM[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
            Instantiate(_playerMoveQueueBM[0].mainMoveGameObject,currentActiveCharacter.transform.position,Quaternion.identity);
            _playerMoveQueueBM.RemoveAt(0);
        }
    }
    public void PlayerAttackFailure()
    {
        _playerMoveQueueBM.Clear();
    }
}

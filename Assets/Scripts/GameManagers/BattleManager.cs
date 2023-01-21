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
    
    public Queue<CharacterGameEntity> characterTurnQueue = new Queue<CharacterGameEntity>();
    
    [SerializeField] GameObject _relativeGround; // <-- The position of this will need to constantly change to wherever the "floor" is for the current target.
    [SerializeField] List<BattleMove> _moveQueue = new List<BattleMove>();

    public enum BattleManagerState
    {
        DEFAULT,
        ANALYSIS, // <-- to implement later. this will be the phase between phases to do auxillary tasks (play animations, wait, etc)
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

        
        
        _relativeGround = FindObjectByName("RelativeGround");
        
        currentActiveCharacter = playerCharacters[0]; // <-- testing 
    }
    GameObject FindObjectByName(string name)
    {
        for (int i = 0; i < ChildObjects.Count; i++)
        {
            if (ChildObjects[i].name == name)
                return ChildObjects[i];
        }
        return null;
    }
    // Feed the move queue into the battle manager to start attacking.
    public void FeedPlayerMoveQueue(List<BattleMove> playerMoves, CharacterGameEntity targetEnemy)
    {
        _moveQueue = new List<BattleMove>(playerMoves);
        for (int i = 0; i < _moveQueue.Count; i++)
        {
            if (_moveQueue[i].mainMoveGameObject == null)
            {
                Debug.Log("ERROR : " + _moveQueue[i].moveName + " has no hitbox! Aborting.");
                break;
            }
            _moveQueue[i].SetupMainMoveGameObject(targetEnemy,_moveQueue[i],this);
            _moveQueue[i].mainMoveGameObject.transform.position = currentActiveCharacter.transform.position;
        }
    }
    // start attacking.
    public void StartAttack()
    {
        _moveQueue[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
        _relativeGround.transform.position = transform.position - new Vector3(0,currentActiveCharacter.GetComponent<BoxCollider2D>().size.y,0);

        Instantiate(_moveQueue[0].mainMoveGameObject,currentActiveCharacter.transform.position,Quaternion.identity);
        _moveQueue.RemoveAt(0);
    }
    public void PlayerAttackSuccess()
    {
        if (_moveQueue.Count == 0)
            Debug.Log("Success!");
        else
        {
            _moveQueue[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
            Instantiate(_moveQueue[0].mainMoveGameObject,currentActiveCharacter.transform.position,Quaternion.identity);
            _moveQueue.RemoveAt(0);
        }
    }
    public void PlayerAttackFailure()
    {
        _moveQueue.Clear();
    }
}

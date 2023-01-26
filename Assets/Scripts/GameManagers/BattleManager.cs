using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

public class BattleManager : GameManager
{
    /*
        TODO : Make sure that AttackSuccess() is unbiased.
    */
    public List<ItemData> playerItems = new List<ItemData>();
    public List<BattleMove> playerMoves = new List<BattleMove>();
    public List<CharacterGameBattleEntity> playerCharacters = new List<CharacterGameBattleEntity>();
    public List<CharacterGameBattleEntity> enemyCharacters = new List<CharacterGameBattleEntity>();
    public CharacterGameBattleEntity currentActiveCharacter;
    public CharacterGameBattleEntity currentTargetCharacter;    
    public static BattleManagerState CurrentBattleManagerState;

    Timer timer = new Timer(1); // <-- timer currently just used for testing. 
    [SerializeField] List<BattleMove> _moveQueue = new List<BattleMove>();
    [SerializeField] GameObject _PlayerUI;
    
    public enum BattleManagerState
    {
        DEFAULT,
        ANALYSIS, // <-- this state is only in effect in between turns and is what ultimately decides the next turn.
        PLAYERTURN,
        PLAYERATTACK,
        ENEMYTURN
    }

    void Awake()
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
            case (BattleManagerState.ANALYSIS) :
            {
                AnaylizeGameState();
                Debug.Log("Analysis");
                break;
            }
            case (BattleManagerState.PLAYERTURN) :
            {
                EnablePlayerUI();
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
                EnemyTurnState();
                Debug.Log("EnemyTurn");
                break;
            }
        }
    }

    void EnablePlayerUI()
    {
        if(_PlayerUI)
        {
            _PlayerUI.SetActive(true);
        }
    }

    void AnaylizeGameState()
    {
        bool flag = false;

        // wait for every character to reach their start position.
        for (int i = 0; i < CharacterGameObjects.Count; i++)
        {
            // character is dead.
            if (CharacterGameObjects[i].characterData.curHP <= 0 && 
                    CharacterGameObjects[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY) 
            { flag = true; CharacterGameObjects[i].KillCharacterInBattle(); Debug.Log("I killed you, HA!"); break; }

            // a character has been hit and is either in hitstun or recovering from it.
            if (CharacterGameObjects[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.HITSTUN ||
                    CharacterGameObjects[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY)
            { flag = true; break; }
        }
        if (!flag)
            GetNextTurn();
    }
    void EnemyTurnState()
    {
        // make sure the current active character is an enemy.
        if (currentActiveCharacter != null && currentActiveCharacter.characterData.CharType == CharacterBase.CharacterType.ENEMY)
        {
            currentActiveCharacter.GetComponent<BasicEnemyAI>().Execute();
        }
    }
    // initialize all entities and values in scene.
    void BattleManagerSetup()
    {
        CentralManager.SetStateManager(this);
        CharacterGameObjects = new List<CharacterGameBattleEntity>(FindObjectsOfType<CharacterGameBattleEntity>()); 
        
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

        _PlayerUI = FindObjectByName("PlayerTurnUI");

        SetTurnOrder();

        currentActiveCharacter = playerCharacters[0]; // <-- testing. really there should be a little bit of a wait before starting, but this works fine.
    }
    void DetermineStateBasedOnActiveCharacter()
    {
        switch(currentActiveCharacter.characterData.CharType) 
        {
            case (CharacterBase.CharacterType.ENEMY) :
            {   CurrentBattleManagerState = BattleManagerState.ENEMYTURN; break; }
            
            case (CharacterBase.CharacterType.PLAYER) :
            {   CurrentBattleManagerState = BattleManagerState.PLAYERTURN; break; }
        }
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

    ///<summary>
    ///Feed the battle manager the moves to perform.
    ///</summary>                                                            
    public void FeedMoveQueue(List<BattleMove> playerMoves, CharacterGameBattleEntity targetEnemy)
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

        Instantiate(_moveQueue[0].mainMoveGameObject,currentActiveCharacter.transform.position,Quaternion.identity);
        _moveQueue.RemoveAt(0);
    }

    // the player has successfully performed a move/portion of their move. advance to next move (if any)
    public void AttackSuccess()
    {
        if (_moveQueue.Count == 0)
        {
            Debug.Log("Success!");
            currentTargetCharacter = null;
            CurrentBattleManagerState = BattleManagerState.ANALYSIS;
        }
        else
        {
            _moveQueue[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
            Instantiate(_moveQueue[0].mainMoveGameObject,currentActiveCharacter.transform.position,Quaternion.identity);
            _moveQueue.RemoveAt(0);
        }
    }
    
    // when the player fails their move, this function should be called. 
    public void PlayerAttackFailure()
    {
        _moveQueue.Clear();
        CurrentBattleManagerState = BattleManagerState.ANALYSIS;
    }


    // sort the current player queue and decide the turn order. (selection sort)
    public void SetTurnOrder()
    {
        int minIdx;
        for (int i = 0; i < CharacterGameObjects.Count; i++)
        {
            minIdx = i;
            for (int j = 0; j < CharacterGameObjects.Count; j++)
            {
                if (CharacterGameObjects[j].characterData.curSPEED < CharacterGameObjects[minIdx].characterData.curSPEED)
                    minIdx = j;
                if (minIdx != i)
                {
                    CharacterGameBattleEntity t = CharacterGameObjects[minIdx];
                    CharacterGameObjects[minIdx] = CharacterGameObjects[i];
                    CharacterGameObjects[i] = t;
                }
            }
        }
        currentActiveCharacter = CharacterGameObjects[0];
        DetermineStateBasedOnActiveCharacter();
    }
    // fetch the next active character from the turn queue. 
    public void GetNextTurn()
    {
        if (CharacterGameObjects.Count > 1 && enemyCharacters.Count > 0)
        {
            currentActiveCharacter = CharacterGameObjects[1];
            List<CharacterGameBattleEntity> t = new List<CharacterGameBattleEntity>(CharacterGameObjects);
            t.RemoveAt(0);
            t.Add(CharacterGameObjects[0]);
            CharacterGameObjects = t;
            DetermineStateBasedOnActiveCharacter();
        }
        else
        {
            Debug.Log("You Won!");
            CurrentBattleManagerState = BattleManagerState.DEFAULT; 
        }

        timer.SetTimer(1); // tester
    }
}

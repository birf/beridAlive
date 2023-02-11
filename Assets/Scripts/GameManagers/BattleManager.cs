using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeriUtils.Core;

public class BattleManager : GameManager
{
    public List<ItemData> playerItems = new List<ItemData>();
    public List<BattleMove> playerMoves = new List<BattleMove>();
    public List<CharacterGameBattleEntity> playerCharacters = new List<CharacterGameBattleEntity>();
    public List<CharacterGameBattleEntity> enemyCharacters = new List<CharacterGameBattleEntity>();
    public CharacterGameBattleEntity currentActiveCharacter;
    public CharacterGameBattleEntity currentTargetCharacter;
    public static BattleManagerState CurrentBattleManagerState;

    [SerializeField] List<BattleMove> _moveQueue = new List<BattleMove>();
    [SerializeField] GameObject _PlayerUI;
    Timer startupTimer = new Timer(1);
    Timer endTimer = new Timer(1);

    public enum BattleManagerState
    {
        DEFAULT,
        ANALYSIS, // <-- this state is only in effect in between turns and is what ultimately decides the next turn.
        PLAYERTURN,
        PLAYERATTACK,
        ENEMYTURN,
        WIN,
        LOSE
    }

    void Awake()
    {
        CurrentBattleManagerState = BattleManagerState.DEFAULT;
        CentralManager.CurrentContext = CentralManager.Context.BATTLE;
        startupTimer.OnTimerEnd += BattleManagerSetup;
        endTimer.OnTimerEnd += RestartGame;
    }
    void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        // done to make sure that all characters on field are initialized with correct values
        if (startupTimer.GetRemaingingSeconds() > 0)
            startupTimer.Tick(Time.deltaTime);
            
        switch (CurrentBattleManagerState)
        {
            case (BattleManagerState.DEFAULT):
                {
                    break;
                }
            case (BattleManagerState.ANALYSIS):
                {
                    AnaylizeGameState();
                    break;
                }
            case (BattleManagerState.PLAYERTURN):
                {
                    EnablePlayerUI();
                    break;
                }
            case (BattleManagerState.PLAYERATTACK):
                {
                    break;
                }
            case (BattleManagerState.ENEMYTURN):
                {
                    EnemyTurnState();
                    break;
                }
            case BattleManagerState.LOSE :
            case BattleManagerState.WIN :
            {
                endTimer.Tick(Time.deltaTime);
                break;
            }
            
        }
    }

    void EnablePlayerUI()
    {
        if (_PlayerUI)
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
            { flag = true; CharacterGameObjects[i].KillCharacterInBattle(); break; }

            // a character has been hit and is either in hitstun or recovering from it.
            if (CharacterGameObjects[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.HITSTUN ||
                    CharacterGameObjects[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY)
            { flag = true; break; }

            if (playerCharacters.Count == 0)
            { flag = true; CurrentBattleManagerState = BattleManagerState.LOSE; Debug.Log("You lose!"); break;} 

            if (enemyCharacters.Count == 0)
            { flag = true; CurrentBattleManagerState = BattleManagerState.WIN; Debug.Log("You Win!"); break;}
            
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
    /*
        Note that this entire function will need to be rewritten when actually starting a battle. 
        This function relies on all objects already being present and active.
    */
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

        currentActiveCharacter = CharacterGameObjects[0]; // <-- testing. really there should be a little bit of a wait before starting, but this works fine.
    }
    void DetermineStateBasedOnActiveCharacter()
    {
        Debug.Log("determining active character");
        Debug.Log(currentActiveCharacter.characterData.CharType);
        switch (currentActiveCharacter.characterData.CharType)
        {
            case (CharacterBase.CharacterType.ENEMY):
                { CurrentBattleManagerState = BattleManagerState.ENEMYTURN; break; }

            case (CharacterBase.CharacterType.PLAYER):
                { CurrentBattleManagerState = BattleManagerState.PLAYERTURN; break; }
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
            _moveQueue[i].SetupMainMoveGameObject(targetEnemy, _moveQueue[i], this);
            _moveQueue[i].mainMoveGameObject.transform.position = currentActiveCharacter.transform.position;
        }
    }
    // start attacking.
    public void StartAttack()
    {
        _moveQueue[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();

        Instantiate(_moveQueue[0].mainMoveGameObject, currentActiveCharacter.transform.position, Quaternion.identity);
        _moveQueue.RemoveAt(0);
    }

    // the player or enemy has successfully performed a move/portion of their move. advance to next move (if any)
    public void AttackSuccess()
    {
        if (_moveQueue.Count == 0)
        {
            currentTargetCharacter = null;
            CurrentBattleManagerState = BattleManagerState.ANALYSIS;
        }
        // if the target character died from the previous move, force an attack chain to succeed, refund the player's stamina
        else if (_moveQueue.Count > 0 && currentTargetCharacter.characterData.curHP <= 0)
        {
            for (int i = 0; i < _moveQueue.Count; i++)
                currentActiveCharacter.characterData.curSTAMINA += _moveQueue[i].staminaCost;
            currentTargetCharacter = null;
            CurrentBattleManagerState = BattleManagerState.ANALYSIS;
            _moveQueue.Clear();
        }
        else // feed next move into the queue.
        {
            _moveQueue[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
            Instantiate(_moveQueue[0].mainMoveGameObject, currentActiveCharacter.transform.position, Quaternion.identity);
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
        if (CharacterGameObjects.Count == 0)
            return;
        currentActiveCharacter = CharacterGameObjects[1];
        List<CharacterGameBattleEntity> t = new List<CharacterGameBattleEntity>(CharacterGameObjects);
        t.RemoveAt(0);
        t.Add(CharacterGameObjects[0]);
        CharacterGameObjects = t;
        DetermineStateBasedOnActiveCharacter();
    }
}

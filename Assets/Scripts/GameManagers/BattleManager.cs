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
    public List<CharacterGameBattleEntity> CharacterGameBattleEntities = new List<CharacterGameBattleEntity>();

    public static BattleManagerState CurrentBattleManagerState;

    [SerializeField] List<BattleMove> _moveQueue = new List<BattleMove>();
    [SerializeField] GameObject _PlayerUI;
    Timer startupTimer;
    Timer endTimer;

    #region Spawn Positions

    Vector2[][] spawnPositions = {
        new Vector2[] {new Vector2(4.5f,0.75f)},
        new Vector2[] {new Vector2(6.25f,3f), new Vector2(5f,-2f)},
        new Vector2[] {new Vector2(6.25f,3f), new Vector2(5f,-2f)  , new Vector2(8.5f,0.75f)},
        new Vector2[] {new Vector2(4.75f,3f), new Vector2(8.75f,3f), new Vector2(3.25f,-2f), new Vector2(8f,-2f)}
    };
    #endregion

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

    void OnEnable()
    {
        startupTimer = new Timer(1);
        endTimer = new Timer(1);
        CurrentBattleManagerState = BattleManagerState.DEFAULT;
        CentralManager.CurrentContext = CentralManager.Context.BATTLE;
        CentralManager.SetStateManager(this);
        startupTimer.OnTimerEnd += BattleManagerSetup;
        endTimer.OnTimerEnd += ReturnToOverworld;
    }
    void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    void ReturnToOverworld()
    {
        OverworldManager om = ChildObjects[0].GetComponent<OverworldManager>();
        if (CurrentBattleManagerState == BattleManagerState.WIN)
        {
            Destroy(om.currentEnemyEncounter.gameObject);
            om.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
            RestartGame();
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
            case BattleManagerState.LOSE:
            case BattleManagerState.WIN:
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
        for (int i = 0; i < CharacterGameBattleEntities.Count; i++)
        {
            // character is dead.
            if (CharacterGameBattleEntities[i].characterData.curHP <= 0 &&
                    CharacterGameBattleEntities[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY)
            { flag = true; CharacterGameBattleEntities[i].KillCharacterInBattle(); break; }

            // a character has been hit and is either in hitstun or recovering from it.
            if (CharacterGameBattleEntities[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.HITSTUN ||
                    CharacterGameBattleEntities[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY)
            { flag = true; break; }

            if (playerCharacters.Count == 0)
            { flag = true; CurrentBattleManagerState = BattleManagerState.LOSE; Debug.Log("You lose!"); break; }

            if (enemyCharacters.Count == 0)
            { flag = true; CurrentBattleManagerState = BattleManagerState.WIN; Debug.Log("You Win!"); break; }

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
        This function relies on all objects already being present.
    */
    void BattleManagerSetup()
    {
        CentralManager.SetStateManager(this);

        CharacterGameBattleEntities = new List<CharacterGameBattleEntity>(FindObjectsOfType<CharacterGameBattleEntity>());









        for (int i = 0; i < CharacterGameBattleEntities.Count; i++)
        {
            // if the character type is the player, setup their moves and items. 
            CharacterGameBattleEntities[i].characterSelectable.isDestroyable = false;
            CharacterGameBattleEntities[i].characterSelectable.canBeDisabled = false;

            if (CharacterGameBattleEntities[i].characterData.CharType == CharacterBase.CharacterType.PLAYER)
            {
                // playerItems = new List<ItemData>(CharacterGameBattleEntities[i].characterScriptable.characterItems);
                playerItems = CharacterGameBattleEntities[i].characterScriptable.characterItems;
                playerMoves = new List<BattleMove>(CharacterGameBattleEntities[i].characterScriptable.characterMoves);
                playerCharacters.Add(CharacterGameBattleEntities[i]);
                CharacterGameBattleEntities[i].characterSelectable.cyclableElements += 1;
            }

            if (CharacterGameBattleEntities[i].characterData.CharType == CharacterBase.CharacterType.ENEMY)
            {
                enemyCharacters.Add(CharacterGameBattleEntities[i]);
                CharacterGameBattleEntities[i].characterSelectable.cyclableElements += 1;
            }
        }

        _PlayerUI = FindObjectByName("PlayerTurnUI");

        SetTurnOrder();

        currentActiveCharacter = CharacterGameBattleEntities[0];
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
        for (int i = 0; i < CharacterGameBattleEntities.Count; i++)
        {
            minIdx = i;
            for (int j = 0; j < CharacterGameBattleEntities.Count; j++)
            {
                if (CharacterGameBattleEntities[j].characterData.curSPEED < CharacterGameBattleEntities[minIdx].characterData.curSPEED)
                    minIdx = j;
                if (minIdx != i)
                {
                    CharacterGameBattleEntity t = CharacterGameBattleEntities[minIdx];
                    CharacterGameBattleEntities[minIdx] = CharacterGameBattleEntities[i];
                    CharacterGameBattleEntities[i] = t;
                }
            }
        }
        currentActiveCharacter = CharacterGameBattleEntities[0];
        DetermineStateBasedOnActiveCharacter();
    }
    // fetch the next active character from the turn queue. 
    public void GetNextTurn()
    {
        if (CharacterGameBattleEntities.Count == 0)
            return;
        currentActiveCharacter = CharacterGameBattleEntities[1];
        List<CharacterGameBattleEntity> t = new List<CharacterGameBattleEntity>(CharacterGameBattleEntities);
        t.RemoveAt(0);
        t.Add(CharacterGameBattleEntities[0]);
        CharacterGameBattleEntities = t;
        DetermineStateBasedOnActiveCharacter();
    }
}


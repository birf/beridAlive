using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeriUtils.Core;

public class BattleManager : GameManager
{

    /*
        TODO : Implement a way to have the battle manager know which move was performed previously.
            NOTE : Simply setting a "previousMoveType" from when we feed the moves into the queue WILL NOT WORK. 
    */

    public int battleManagerMoveQueueIndex = 0;

    public List<ItemData> playerItems = new List<ItemData>();
    public List<BattleMove> playerMoves = new List<BattleMove>();
    public List<CharacterGameBattleEntity> playerCharacters = new List<CharacterGameBattleEntity>();
    public List<CharacterGameBattleEntity> enemyCharacters = new List<CharacterGameBattleEntity>();
    public CharacterGameBattleEntity currentActiveCharacter;
    public CharacterGameBattleEntity currentTargetCharacter;
    public List<CharacterGameBattleEntity> CharacterGameBattleEntities = new List<CharacterGameBattleEntity>();

    public static BattleManagerState CurrentBattleManagerState;

    public List<BattleMove> battleManagerMoveQueue = new List<BattleMove>();
    public Timer waitTimer;
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
        WAIT,
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void ReturnToOverworld()
    {
        OverworldManager om = ChildObjects[0].GetComponent<OverworldManager>();

        //Zay's Code
        //
        if (CurrentBattleManagerState == BattleManagerState.WIN)
        {
            currentActiveCharacter.characterData.curSTAMINA = currentActiveCharacter.characterData.baseSTAMINA;
            FindObjectOfType<AudioManager>().EndTrack();
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
            case (BattleManagerState.WAIT) :
                {
                    waitTimer.Tick(Time.deltaTime);
                    break;
                }
            case (BattleManagerState.ANALYSIS):
                {
                    AnalyzeGameState();
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

    void AnalyzeGameState()
    {
        bool flag = false;

        // wait for every character to reach their start position.
        for (int i = 0; i < CharacterGameBattleEntities.Count; i++)
        {
            // character is dead.
            // if (CharacterGameBattleEntities[i].characterData.curHP <= 0 )
            // { flag = true; CharacterGameBattleEntities[i].KillCharacterInBattle(); break; }

            // a character has been hit and is either in hitstun or recovering from it.
            if (CharacterGameBattleEntities[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.HITSTUN
            ||  CharacterGameBattleEntities[i].characterBattlePhysics.characterPhysicsState == BattlePhysicsInteraction.CharacterPhysicsState.RECOVERY)
            { flag = true; break; }

            if (playerCharacters.Count == 0)
            { flag = true; CurrentBattleManagerState = BattleManagerState.LOSE; Debug.Log("You lose!"); break; }

            if (enemyCharacters.Count == 0)
            { flag = true; CurrentBattleManagerState = BattleManagerState.WIN; Debug.Log("You Win!"); break; }

        }
        if (!flag)
        {
            if (waitTimer == null)
            {
                waitTimer = new Timer(1);
                waitTimer.OnTimerEnd += GetNextTurn;
            }
            CurrentBattleManagerState = BattleManagerState.WAIT;
        }
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

        CharacterGameBattleEntities = new List<CharacterGameBattleEntity>(FindObjectsOfType<CharacterGameBattleEntity>());

        for (int i = 0; i < CharacterGameBattleEntities.Count; i++)
        {
            // if the character type is the player, setup their moves and items. 
            CharacterGameBattleEntities[i].characterSelectable.isDestroyable = false;
            CharacterGameBattleEntities[i].characterSelectable.canBeDisabled = false;

            if (CharacterGameBattleEntities[i].characterData.CharType == CharacterBase.CharacterType.PLAYER)
            {
                playerItems = new List<ItemData>(CharacterGameBattleEntities[i].characterScriptable.characterItems);
                playerMoves = new List<BattleMove>(CharacterGameBattleEntities[i].characterScriptable.characterMoves);
                playerCharacters.Add(CharacterGameBattleEntities[i]);
                CharacterGameBattleEntities[i].characterSelectable.cyclableElements += 1;
            }

            if (CharacterGameBattleEntities[i].characterData.CharType == CharacterBase.CharacterType.ENEMY)
            {
                enemyCharacters.Add(CharacterGameBattleEntities[i]);
                CharacterGameBattleEntities[i].characterSelectable.cyclableElements += 1;
            }
            Characters.Add(CharacterGameBattleEntities[i].characterData);
        }

        _PlayerUI = FindObjectByName("PlayerTurnUI");


        SetTurnOrder();
    }
    void DetermineStateBasedOnActiveCharacter()
    {
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
        battleManagerMoveQueue = new List<BattleMove>(playerMoves);
        for (int i = 0; i < battleManagerMoveQueue.Count; i++)
        {
            if (battleManagerMoveQueue[i].mainMoveGameObject == null)
            {
                Debug.Log("ERROR : " + battleManagerMoveQueue[i].moveName + " has no hitbox! Aborting.");
                break;
            }

            battleManagerMoveQueue[i].SetupMainMoveGameObject(targetEnemy, battleManagerMoveQueue[i], this);

            battleManagerMoveQueue[i].mainMoveGameObject.transform.position = currentActiveCharacter.transform.position;
        }
    }
    // start attacking.
    public void StartAttack()
    {
        Instantiate(battleManagerMoveQueue[0].mainMoveGameObject, currentActiveCharacter.transform.position, Quaternion.identity);
        battleManagerMoveQueue[0].mainMoveGameObject.GetComponent<ATKScript>().BeginMove();
    }

    // the player or enemy has successfully performed a move/portion of their move. advance to next move (if any)
    public void AttackSuccess()
    {
        if (currentActiveCharacter.characterData.CharType == CharacterBase.CharacterType.ENEMY)
            currentActiveCharacter.GetComponent<BasicEnemyAI>().canExecute = true;

        // FindObjectOfType<HealthDisplay>().UpdateHealthUI();

        // last move was a success, clear queue.
        if (battleManagerMoveQueue.Count - 1 == battleManagerMoveQueueIndex)
        {
            CurrentBattleManagerState = BattleManagerState.ANALYSIS;
            battleManagerMoveQueueIndex = 0;
            battleManagerMoveQueue.Clear();
        }
        // if the target character died from the previous move, force an attack chain to succeed, refund the player's stamina
        else if (currentTargetCharacter.characterData.curHP <= 0)
        {
            for (int i = battleManagerMoveQueueIndex; i < battleManagerMoveQueue.Count; i++)
                currentActiveCharacter.characterData.curSTAMINA += battleManagerMoveQueue[i].staminaCost;
            CurrentBattleManagerState = BattleManagerState.ANALYSIS;
            battleManagerMoveQueue.Clear();
            battleManagerMoveQueueIndex = 0;
            //FindObjectOfType<HealthDisplay>().RemoveHudEntity(currentActiveCharacter.GetComponent<HudEntity>())
        }
        else // advance to next move in the queue.
        {
            battleManagerMoveQueueIndex++;
            Instantiate(battleManagerMoveQueue[battleManagerMoveQueueIndex].mainMoveGameObject, currentActiveCharacter.transform.position, Quaternion.identity);
        }
    }

    // when the player fails their move, this function should be called. 
    public void PlayerAttackFailure()
    {
        battleManagerMoveQueue.Clear();
        battleManagerMoveQueueIndex = 0;
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

        // if the next turn is the player's turn, add 1 back to their stamina
        if (CurrentBattleManagerState == BattleManagerState.PLAYERTURN)
            currentActiveCharacter.characterData.AddToStat(CharacterStat.STAMINA, 1, false);

        CheckStatusEffects(currentActiveCharacter.characterData);
        waitTimer = null;
    }
    public void CheckStatusEffects(CharacterBase activeChar)
    {
        List<CharacterStatusEffect> curList = new List<CharacterStatusEffect>(activeChar.statusEffects);
        foreach (CharacterStatusEffect statusEffect in curList)
        {
            statusEffect.duration -= 1;
            if (statusEffect.duration == 0)
            {
                statusEffect.RevertChanges(true);
                activeChar.statusEffects.Remove(statusEffect);
            }
        }
    }
}



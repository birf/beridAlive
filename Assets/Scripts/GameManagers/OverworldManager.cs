using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : GameManager
{
    public CharacterGameOverworldEntity currentEnemyEncounter;
    public CharacterGameOverworldEntity playerCharacter;

    [SerializeField] GameObject enemyPrefab;


    [SerializeField] GameObject entities;

    [SerializeField] List<Vector2> battlePositions;

    //[SerializeField] AudioManager audioManager;

    //current level
    private int levelNumber;
    public OverworldLevel activeLevel;
    public List<OverworldLevel> overworldLevels = new List<OverworldLevel>();




    void Start()
    {
        CentralManager.SetStateManager(this);
        CentralManager.CurrentContext = CentralManager.Context.OVERWORLD;
        OverworldManagerSetup();
    }
    void OnEnable()
    {
        CentralManager.CurrentContext = CentralManager.Context.OVERWORLD;
        CentralManager.SetStateManager(this);
    }
    void OnDisable()
    {
        CentralManager.CurrentContext = CentralManager.Context.BATTLE;
        CentralManager.SetStateManager((BattleManager)ChildObjects[0].GetComponent<BattleManager>());
    }

    GameObject CreateEnemy(CharacterScriptable characterScriptable)
    {
        GameObject enemy = Instantiate(enemyPrefab);
        CharacterGameBattleEntity entity = enemy.GetComponent<CharacterGameBattleEntity>();
        entity.characterScriptable = characterScriptable;
        entity.characterAnimations = characterScriptable.charAnimations;
        entity.GetComponent<BattleDropShadow>().SetDropShadowSize(entity.characterScriptable.battleDropShadowSize);
        entity.GetComponentInChildren<HudEnemyHealthBar>().curHealth = characterScriptable.characterData.curHP;
        entity.GetComponentInChildren<HudEnemyHealthBar>().maxHealth= characterScriptable.characterData.baseHP;

        entity.CharacterSetup();
        return enemy;

    }

    public void BattleStart()
    {
        BattleManager bm = ChildObjects[0].GetComponent<BattleManager>();
        FindObjectOfType<AudioManager>().PlayTrack(AUDIOCLIPS.BATTLE_THEME);


        bm.gameObject.SetActive(true);


        for (int i = 0; i < currentEnemyEncounter.partnerCharacters.Count; i++)
        {
            GameObject enemy = CreateEnemy(currentEnemyEncounter.partnerCharacters[i]);
            enemy.transform.localPosition = battlePositions[i];
            enemy.GetComponent<BattlePhysicsInteraction>().startPosition = battlePositions[i];
            enemy.GetComponent<BattlePhysicsInteraction>().localGroundYCoordinate = battlePositions[i].y;
        }

        gameObject.SetActive(false);
    }
    void OverworldManagerSetup()
    {
        CharacterGameOverworldEntity[] characterEntities = FindObjectsOfType<CharacterGameOverworldEntity>();
        for (int i = 0; i < characterEntities.Length; i++)
        {
            Characters.Add(characterEntities[i].characterData);
            if (characterEntities[i].characterData.CharType == CharacterBase.CharacterType.PLAYER)
                playerCharacter = characterEntities[i];
        }
        overworldLevels = new List<OverworldLevel>(FindObjectsOfType<OverworldLevel>(true));
        //enable the first level
        levelNumber = 0;
        nextLevel();


    }


    //load the next level
    // public void nextLevel()
    // {
    //     string[] levelNames = new string[] { "test_level_1", "test_level_2" };
    //     GameObject levels = GameObject.Find("Levels");
    //     if (activeLevel != null) activeLevel.SetActive(false); //deactivate current level
    //     //find and enable level
    //     Debug.Log(levelNames[levelNumber]);
    //     activeLevel = levels.transform.Find(levelNames[levelNumber]).gameObject;
    //     activeLevel.SetActive(true);
    //     activeLevel.GetComponent<OverworldLevel>().initializeLevel();

    //     //add each enemy to Characters and change their parent to entities
    //     foreach (GameObject enemy in activeLevel.GetComponent<OverworldLevel>().getEnemies())
    //     {
    //         Characters.Add(enemy.GetComponent<CharacterGameOverworldEntity>().characterData);
    //         enemy.transform.parent = GameObject.Find("entities").transform;
    //     }
    //     levelNumber++;

    // }

    public void nextLevel()
    {
        if (activeLevel)
            activeLevel.gameObject.SetActive(false);

        activeLevel = overworldLevels[levelNumber];
        activeLevel.gameObject.SetActive(true);
        activeLevel.initializeLevel();

        foreach (GameObject enemy in activeLevel.getEnemies())
        {
            Characters.Add(enemy.GetComponent<CharacterGameOverworldEntity>().characterData);
        }
        levelNumber++;
    }

}

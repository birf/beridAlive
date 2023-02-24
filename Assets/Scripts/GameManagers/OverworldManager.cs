using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : GameManager
{
    public CharacterGameOverworldEntity currentEnemyEncounter;

    [SerializeField] GameObject @object;


    [SerializeField] GameObject entities;

    [SerializeField] List<Vector2> battlePositions;

    //current level
    private int levelNumber; 
    public GameObject activeLevel;

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
        GameObject enemy = Instantiate(@object);
        CharacterGameBattleEntity entity = enemy.GetComponent<CharacterGameBattleEntity>();
        entity.characterScriptable = characterScriptable;
        entity.characterAnimations = characterScriptable.charAnimations;

        enemy.transform.parent = entities.transform;


        return enemy;

    }

    public void BattleStart()
    {
        BattleManager bm = ChildObjects[0].GetComponent<BattleManager>();
        bm.gameObject.SetActive(true);


        for (int i = 0; i < currentEnemyEncounter.partnerCharacters.Count; i++)
        {
            GameObject enemy = CreateEnemy(currentEnemyEncounter.partnerCharacters[i]);
            enemy.transform.localPosition = battlePositions[i];
            enemy.GetComponent<BattlePhysicsInteraction>().startPosition = battlePositions[i];
        }


        gameObject.SetActive(false);
    }
    void OverworldManagerSetup()
    {
        CharacterGameOverworldEntity[] characterEntities = FindObjectsOfType<CharacterGameOverworldEntity>();
        for (int i = 0; i < characterEntities.Length; i++)
        {
            Characters.Add(characterEntities[i].characterData);
        }

        //enable the first level
        levelNumber = 0;
        nextLevel();
    }


    //load the next level
    public void nextLevel(){
        string[] levelNames = new string[]{"test_level_1", "test_level_2"}; 
        GameObject levels = GameObject.Find("Levels");
        if(activeLevel != null) activeLevel.SetActive(false); //deactivate current level
        //find and enable level
        Debug.Log(levelNames[levelNumber]);
        activeLevel = levels.transform.Find(levelNames[levelNumber]).gameObject;
        activeLevel.SetActive(true);
        activeLevel.GetComponent<OverworldLevel>().initializeLevel();

        //add each enemy to Characters and change their parent to entities
        foreach (GameObject enemy in activeLevel.GetComponent<OverworldLevel>().getEnemies()){
            Characters.Add(enemy.GetComponent<CharacterGameOverworldEntity>().characterData);
            enemy.transform.parent = GameObject.Find("entities").transform;      
        }
        levelNumber++;
        

    }

}

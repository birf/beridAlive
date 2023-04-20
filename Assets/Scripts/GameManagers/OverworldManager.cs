using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;
public class OverworldManager : GameManager
{
    public CharacterGameOverworldEntity currentEnemyEncounter;
    public CharacterGameOverworldEntity playerCharacter;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject entities;
    [SerializeField] GameObject battleOpeningMovie;
    [SerializeField] List<Vector2> battlePositions;

    private int levelNumber;
    public Timer generalTimer = new Timer(1);
    public OverworldLevel activeLevel;
    public List<OverworldLevel> overworldLevels = new List<OverworldLevel>();

    void Update()
    {
        generalTimer.Tick(Time.deltaTime);
    }
    void Start()
    {
        CentralManager.SetStateManager(this);
        CentralManager.CurrentContext = CentralManager.Context.OVERWORLD;
        OverworldManagerSetup();
        TogglePlayerController(false);
        generalTimer.OnTimerEnd += Stupid;
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
        bool playBossTheme = false;
        BattleManager bm = ChildObjects[0].GetComponent<BattleManager>();
        bm.gameObject.SetActive(true);
        bm.openingMovie.gameObject.SetActive(true);

        for (int i = 0; i < currentEnemyEncounter.partnerCharacters.Count; i++)
        {
            bm.openingMovie.enemiesToDisplay.Add(currentEnemyEncounter.partnerCharacters[i]);

            if (currentEnemyEncounter.partnerCharacters[i].characterData.CharType == CharacterBase.CharacterType.BOSS ||
                currentEnemyEncounter.partnerCharacters[i].characterData.CharacterName == "Satan")
                playBossTheme = true;

            GameObject enemy = CreateEnemy(currentEnemyEncounter.partnerCharacters[i]);
            enemy.transform.localPosition = battlePositions[i];
            enemy.GetComponent<BattlePhysicsInteraction>().startPosition = battlePositions[i];
            enemy.GetComponent<BattlePhysicsInteraction>().localGroundYCoordinate = battlePositions[i].y;
        }
        if (playBossTheme)
            ChildObjects[3].GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.BOSS_THEME);
        else
            ChildObjects[3].GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.BATTLE_THEME);


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
        levelNumber = 0;
        nextLevel();
    }

    public void TogglePlayerController(bool active = true)
    {
        if (active)
            playerCharacter.GetComponent<OverworldPlayerMove>().controls.Enable();
        else
            playerCharacter.GetComponent<OverworldPlayerMove>().controls.Disable();
    }
    public void TogglePlayerController()
    {
        if (playerCharacter.gameObject.activeSelf)
            playerCharacter.GetComponent<OverworldPlayerMove>().controls.Disable();
        else
            playerCharacter.GetComponent<OverworldPlayerMove>().controls.Enable();
    }
    public void StartNextLevel()
    {
        Debug.Log("being Called");
        if (activeLevel.canExit())
        {
            TogglePlayerController(false);
            generalTimer = new Timer(1);
            ChildObjects[1].GetComponent<Animator>().Play("fadeout");
            generalTimer.Tick(Time.deltaTime);
            generalTimer.OnTimerEnd += ToggleLevelUpScreen;
        }
    }
    public void nextLevel()
    {
        if (activeLevel)
        {
            activeLevel.gameObject.SetActive(false);
            activeLevel.battleBackground.SetActive(false);
        }
        activeLevel = overworldLevels[levelNumber];
        activeLevel.gameObject.SetActive(true);
        activeLevel.initializeLevel();

        foreach (GameObject enemy in activeLevel.getEnemies())
        {
            Characters.Add(enemy.GetComponent<CharacterGameOverworldEntity>().characterData);
        }
        levelNumber++;
    }

    // we are almost done so I justify this incredibly stupid, redundant function.
    public void Stupid()
    {
        TogglePlayerController(true);
    }
    void ToggleLevelUpScreen()
    {
        ChildObjects[2].SetActive(true);
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

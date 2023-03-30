using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;
using TMPro;

public class PlayerTurnUI : MonoBehaviour
{
    /*
        Class for managing the player's UI when it is currently their turn in battle.
    */
    public Vector3[] listSpawnerOrigins = { new Vector3(1.5f, 4.0f, 0), new Vector3(2.25f, 2.75f, 0) }; // offsets for spawning list entries.
    public StaminaBar staminaBar;
    public UISelectable goButton;
    public List<UISelectable> playerActionIcons = new List<UISelectable>();
    public List<UISelectable> currentActiveUIElements = new List<UISelectable>();
    public UISelectable currentActiveUIElement;
    public UISelectable currentSelectedItem; // hack. probably a better way to do this. see itemselection()
    public UIListEntry listEntryPrefab;
    public UICursor cursor;
    public TextMeshPro playerActionIconDisplayText;
    public TextMeshPro descriptionDisplay;
    public BattleManager battleManager;

    public List<BattleMove> playerMoveQueue = new List<BattleMove>();
    public List<Tactics> playerTactics = new List<Tactics>();

    [SerializeField] [Range(1.0f, 25.0f)] float _actionIconSpeed = 12.5f;
    [SerializeField] [Range(0.1f, 2.0f)] float _actionIconXScale = 1.25f;
    [SerializeField] [Range(0.1f, 2.0f)] float _actionIconYScale = 1.25f;


    PrimaryControls controls;
    const float THETA = 2 * Mathf.PI;
    public string currentState;
    [SerializeField]
    string[] _playerTurnUIStates =
    {
        "ActionSelect",             // 0
        "Attack",                   // 1
        "Items",                    // 2
        "Tactics",                  // 3
        "ItemUseSelection",         // 4
        "AttackTargetSelection",    // 5
        "NOSTATE"                   // n - 1 : unused
    };
    [SerializeField] GameObject _ActionIcons;

    #region MonoBehaviour Routines
    void OnEnable()
    {
        controls = new PrimaryControls();
        controls.Enable();
        SetupActionIcons();
        currentState = _playerTurnUIStates[0];
    }
    void OnDisable()
    {
        DisableCursor();
        SetDescriptorActive(false);
    }
    void Update()
    {
        // make absolutely sure that there exists a state manager for this object.
        if (CentralManager.GetStateManager() != null)
            battleManager = (BattleManager)CentralManager.GetStateManager();

        if (currentState == "ActionSelect")
            battleManager.currentActiveCharacter.characterAnimator.Play("battle_select");
        else
            battleManager.currentActiveCharacter.characterAnimator.Play("battle_idle");

        // switch statement for current ui state. 
        switch (currentState)
        {
            case ("ActionSelect"):
                {
                    ActionSelect();
                    break;
                }
            case ("Attack"):
            case ("AttackTargetSelection"):
                {
                    AttackSelection();
                    break;
                }
            case ("Items"):
            case ("ItemUseSelection"):
                {
                    ItemSelection();
                    break;
                }
            case ("Tactics"):
                {
                    TacticsSelection();
                    break;
                }
            default:
                {
                    ClearCurrentSubMenu();
                    SetupActionIcons();
                    break;
                }
        }
        // safeguard. probably should remove.
        if (cursor.enabled)
            cursor.cursorSelectable = currentActiveUIElement;
    }
    #endregion
    #region General Functions
    void EnableCursor(Vector3 startPosition)
    {
        cursor.cursorSelectable = currentActiveUIElement;
        cursor.SetCursorInitialPosition(startPosition);
        cursor.gameObject.SetActive(true);
    }
    void DisableCursor()
    {
        cursor.cursorSelectable = null;
        cursor.gameObject.SetActive(false);
    }
    void InstantiateList(List<IDisplayable> ListEntries, Vector3 spawnerOrigin) // create a list of selectable items from the input list
    {
        if (listEntryPrefab == null)
            Debug.Log("No entry prefab loaded!");
        else
        {
            float yOffset = transform.localScale.y; // spawn each list element in the correct location
            for (int i = 0; i < ListEntries.Count; i++)
            {
                Vector3 target = spawnerOrigin;
                target.y -= (i * yOffset);
                UIListEntry objEntry = Instantiate(listEntryPrefab);
                objEntry.selectable.InitializeValues(i, ListEntries.Count, true, false, true);
                objEntry.selectable.InitializePositions(spawnerOrigin, target);
                objEntry.selectable.displayable = ListEntries[i];
                objEntry.SetDisplayData(ListEntries[i]);
                currentActiveUIElements.Add(objEntry.selectable);
            }
            currentActiveUIElement = currentActiveUIElements[0];
            EnableCursor(currentActiveUIElements[0].cursorTarget + spawnerOrigin);
        }
    }
    void ClearCurrentSubMenu()
    {
        if (currentActiveUIElements.Count > 0)
        {
            for (int i = 0; i < currentActiveUIElements.Count; i++)
            {
                if (currentActiveUIElements[i].isDestroyable)
                    Destroy(currentActiveUIElements[i].gameObject);
                if (currentActiveUIElements[i].canBeDisabled)
                    currentActiveUIElements[i].gameObject.SetActive(false);
            }
        }

        currentActiveUIElements.Clear();


        if (playerActionIconDisplayText.IsActive())
            playerActionIconDisplayText.gameObject.SetActive(false); // stop displaying the text under the active icon
        if (staminaBar.gameObject.activeSelf)
        { staminaBar.gameObject.SetActive(false); staminaBar.currentLerpTime = 0; }

        if (currentState == _playerTurnUIStates[0])
            foreach (UISelectable icon in playerActionIcons)
                icon.transform.position = icon.initialPosition;
    }
    void Navigate(int shift) // Navigate through the current list of active UI elements. 
    {
        int i;
        for (i = 0; i < currentActiveUIElements.Count; i++)
        {
            currentActiveUIElements[i].ShiftCycle(shift);
            if (currentActiveUIElements[i].cycle == 0) { currentActiveUIElement = currentActiveUIElements[i]; }
        }
        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.NAVIGATE);
    }
    #endregion
    #region ActionSelect Methods
    void ActionSelect() // Method for moving the actionIcons.
    {
        SetDescriptorActive(false);

        playerActionIconDisplayText.text = currentActiveUIElement.gameObject.name;
        currentActiveUIElements = new List<UISelectable>(playerActionIcons);
        if (cursor.gameObject.activeSelf)
            DisableCursor();

        if (controls.Battle.Direction.triggered)
        {
            Navigate((int)-controls.Battle.Direction.ReadValue<Vector2>().x);
        }
        for (int i = 0; i < playerActionIcons.Count; i++)
            SmoothMoveActionIcon(i);

        if (controls.Battle.Primary.triggered && currentActiveUIElement.isSelectable)
        {
            switch (currentActiveUIElement.name)
            {
                case "Attack":
                    {
                        ClearCurrentSubMenu();
                        currentState = _playerTurnUIStates[1];
                        InstantiateList(new List<IDisplayable>(battleManager.playerMoves), listSpawnerOrigins[0] + transform.position);
                        for (int i = 0; i < currentActiveUIElements.Count; i++)
                        {
                            currentActiveUIElements[i].cyclableElements += 1;
                        }
                        currentActiveUIElements.Add(goButton);
                        goButton.InitializeValues(currentActiveUIElements.Count - 1, currentActiveUIElements.Count, false, false, true);

                        break;
                    }
                case "Items":
                    {
                        if (battleManager.playerItems.Count == 0)
                            break;
                        ClearCurrentSubMenu();
                        currentState = _playerTurnUIStates[2];
                        InstantiateList(new List<IDisplayable>(battleManager.playerItems), listSpawnerOrigins[0] + transform.position);
                        break;
                    }
                case "Tactics":
                    {
                        ClearCurrentSubMenu();
                        currentState = _playerTurnUIStates[3];
                        InstantiateList(new List<IDisplayable>(playerTactics),listSpawnerOrigins[0] + transform.position);
                        break;
                    }
            }

            GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.SELECT);
        }
    }
    // initialize action icons. 
    void SetupActionIcons()
    {
        int i;
        for (i = 0; i < playerActionIcons.Count; i++)
        {
            playerActionIcons[i].gameObject.SetActive(true);
            playerActionIcons[i].cycle = i;
            playerActionIcons[i].cyclableElements = playerActionIcons.Count;
        }
        playerActionIconDisplayText.gameObject.SetActive(true);
        currentActiveUIElement = playerActionIcons[0];
    }
    // smoothly move the player icon at index i to appropriate position. [explicitly for actionselection menu] 
    void SmoothMoveActionIcon(int i)
    {
        UISelectable icon = playerActionIcons[i];

        _ActionIcons.transform.position = battleManager.currentActiveCharacter.transform.position + listSpawnerOrigins[1];
        icon.initialPosition = _ActionIcons.transform.position;

        icon.GetComponent<SpriteRenderer>().sortingOrder = icon.cycle == 0 ? 101 : 100 - (icon.cycle);
        icon.GetComponent<SpriteRenderer>().color = icon.cycle == 0 ? Color.white : Color.grey;
        icon.transform.localScale = icon.cycle == 0 ? new Vector3(0.4f, 0.4f, 1) : new Vector3(0.35f, 0.35f, 1);

        Vector3 targetPosition = icon.initialPosition;
        targetPosition.x = icon.initialPosition.x + _actionIconXScale * (Mathf.Sin((THETA * icon.cycle) / icon.cyclableElements));
        targetPosition.y = icon.initialPosition.y + _actionIconYScale * (Mathf.Sin((THETA * icon.cycle) / icon.cyclableElements));

        icon.transform.position = Vector2.Lerp(icon.transform.position, targetPosition, Time.deltaTime * _actionIconSpeed);
    }
    #endregion
    #region AttackMenu Methods
    void AttackSelection()
    {
        ref int curStamina = ref battleManager.currentActiveCharacter.characterData.curSTAMINA;

        if (currentState == _playerTurnUIStates[1])
        {
            staminaBar.staminaCount = battleManager.currentActiveCharacter.characterData.baseSTAMINA;
            goButton.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(true);
            staminaBar.targetPosition = transform.position + new Vector3(-10, 0, 0);
        }

        // draw the stamina bar
        {
            if (staminaBar.gameObject.activeSelf)
            {
                staminaBar.UpdateStaminaBar(curStamina);
            }
        }

        if (playerMoveQueue.Count > 0)
            goButton.isSelectable = true;
        else
            goButton.isSelectable = false;

        if (controls.Battle.Direction.triggered)
        {
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        }

        switch (currentState)
        {
            case ("Attack"): // player is selecting what moves they are going to attack with
                {
                    // display description text.
                    if (currentActiveUIElement.displayable != null)
                    {
                        currentActiveUIElement.displayable.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
                        SetDescriptionText(strings[1]);
                        SetDescriptorActive(true);
                    }
                    else
                        SetDescriptorActive(false);

                    if (controls.Battle.Primary.triggered && currentActiveUIElement.isSelectable)
                    {
                        if (currentActiveUIElement.displayable != null) // if this is a valid element and the current stamina level is high enough
                        {
                            BattleMove current = (BattleMove)currentActiveUIElement.displayable;
                            if (curStamina - current.staminaCost >= 0)
                            {
                                if (playerMoveQueue.Count > 0)
                                {
                                    if (!playerMoveQueue[playerMoveQueue.Count - 1].isFinisher)
                                    {
                                        playerMoveQueue.Add((BattleMove)currentActiveUIElement.displayable);
                                        curStamina -= current.staminaCost;    
                                    }
                                }
                                else
                                {
                                    playerMoveQueue.Add((BattleMove)currentActiveUIElement.displayable);
                                    curStamina -= current.staminaCost;
                                }
                            }
                        }
                        // player selected moves to perform, select target.
                        if (currentActiveUIElement.gameObject.name == "GO!" && playerMoveQueue.Count > 0)
                        {
                            currentState = _playerTurnUIStates[5];
                            ClearCurrentSubMenu();
                            for (int i = 0; i < battleManager.enemyCharacters.Count; i++)
                            {
                                battleManager.enemyCharacters[i].characterSelectable.cycle = i;
                                battleManager.enemyCharacters[i].characterSelectable.cyclableElements = battleManager.enemyCharacters.Count;
                                battleManager.enemyCharacters[i].characterSelectable.index = i;

                                currentActiveUIElements.Add(battleManager.enemyCharacters[i].characterSelectable);
                            }
                            currentActiveUIElement = currentActiveUIElements[0];
                            GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.SELECT);
                        }
                    }

                    if (controls.Battle.Secondary.triggered)
                    {
                        if (playerMoveQueue.Count > 0)
                        {
                            curStamina += playerMoveQueue[playerMoveQueue.Count - 1].staminaCost;
                            playerMoveQueue.RemoveAt(playerMoveQueue.Count - 1);
                            if (playerMoveQueue.Count == 0)
                                goButton.isSelectable = false;

                        }
                        else if (playerMoveQueue.Count == 0)
                        {
                            currentState = _playerTurnUIStates[0];

                            ClearCurrentSubMenu();
                            SetupActionIcons();
                        }
                        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.DESELECT);
                    }
                    break;
                }
            case ("AttackTargetSelection"):
                {

                    SetDescriptorActive(false);

                    if (controls.Battle.Primary.triggered) // player has selected their moves and is ready to start.
                    {

                        battleManager.FeedMoveQueue(playerMoveQueue, currentActiveUIElement.GetComponent<CharacterGameBattleEntity>());
                        playerMoveQueue.Clear();

                        BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.PLAYERATTACK;
                        ClearCurrentSubMenu();
                        currentState = _playerTurnUIStates[6];


                        battleManager.StartAttack();

                        gameObject.SetActive(false);
                    }
                    if (controls.Battle.Secondary.triggered) // cancel enemy selection
                    {
                        currentState = _playerTurnUIStates[1];

                        ClearCurrentSubMenu();

                        InstantiateList(new List<IDisplayable>(battleManager.playerMoves), listSpawnerOrigins[0] + transform.position);

                        for (int i = 0; i < currentActiveUIElements.Count; i++)
                            currentActiveUIElements[i].cyclableElements += 1;

                        currentActiveUIElements.Add(goButton);
                        goButton.InitializeValues(currentActiveUIElements.Count - 1, currentActiveUIElements.Count, false, false, true);
                        currentActiveUIElement = currentActiveUIElements[0];
                        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.DESELECT);
                    }
                    break;
                }
        }
    }
    #endregion
    #region ItemSelection methods
    void ItemSelection()
    {
        if (currentActiveUIElement.displayable != null)
        {
            currentActiveUIElement.displayable.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
            SetDescriptionText(strings[1]);
            SetDescriptorActive(true);
        }

        if (controls.Battle.Direction.triggered)
        {
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        }
        switch (currentState)
        {
            case ("Items"): // Menu selections for selecting an item. 
                {
                    if (controls.Battle.Primary.triggered && currentActiveUIElement.isSelectable) // if the player selected an item, setup the next menu.
                    {
                        // -----------------------------------------------------------------------------------------------------------------------------
                        // | there's probably a better way to do this. since the active ui element would need to serve two purposes, we can't remove   |
                        // V the item after using it by getting the index of the currently active selectable. thus, the selected item must be stored.  V
                        // -----------------------------------------------------------------------------------------------------------------------------
                        currentSelectedItem = currentActiveUIElement;
                        // -----------------------------------------------------------------------------------------------------------------------------
                        ClearCurrentSubMenu();
                        currentState = _playerTurnUIStates[4];
                        for (int i = 0; i < battleManager.playerCharacters.Count; i++)
                        {
                            battleManager.playerCharacters[i].characterSelectable.cycle = i;
                            battleManager.playerCharacters[i].characterSelectable.cyclableElements = battleManager.playerCharacters.Count;
                            battleManager.playerCharacters[i].characterSelectable.index = i;

                            currentActiveUIElements.Add(battleManager.playerCharacters[i].characterSelectable);
                        }
                        currentActiveUIElement = currentActiveUIElements[0];
                        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.SELECT);
                        SetDescriptorActive(false);
                    }
                    if (controls.Battle.Secondary.triggered)
                    {
                        currentState = _playerTurnUIStates[0];
                        ClearCurrentSubMenu();
                        SetupActionIcons();
                        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.DESELECT);
                    }

                    SetDescriptorActive(true);

                    break;
                }
            case ("ItemUseSelection"): // player selected an item to use.
                {
                    SetDescriptorActive(false);

                    if (controls.Battle.Primary.triggered && currentActiveUIElement.isSelectable)
                    {
                        CharacterBase curChar = battleManager.currentActiveCharacter.characterData;
                        ItemData t = (ItemData)currentSelectedItem.displayable;
                        // only use an item if the value you are trying to change is actually different than it's base max value. 
                        if ((curChar.GetStatByStatType(t.statAffected,false) != curChar.GetStatByStatType(t.statAffected,true)) || t.inflictsStatusEffect)   
                        {
                            t.UseItem(curChar, t.statAffected);
                            battleManager.playerItems.RemoveAt(currentSelectedItem.index);
                            ClearCurrentSubMenu();
                            currentState = _playerTurnUIStates[0];
                            SetupActionIcons();
                            currentSelectedItem = null;
                        }
                    }
                    if (controls.Battle.Secondary.triggered)
                    {
                        currentState = _playerTurnUIStates[2];
                        ClearCurrentSubMenu();
                        InstantiateList(new List<IDisplayable>(battleManager.playerItems), listSpawnerOrigins[0] + transform.position);
                        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.DESELECT);
                    }
                    break;
                }
        }
    }
    #endregion
    #region TacticsMenu Methods


    void TacticsSelection()
    {
        SetDescriptorActive(true);
        if (currentActiveUIElement.displayable != null)
        {
            currentActiveUIElement.displayable.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
            SetDescriptionText(strings[1]);
            SetDescriptorActive(true);
        }
        if (controls.Battle.Secondary.triggered)
        {
            currentState = _playerTurnUIStates[0];
            ClearCurrentSubMenu();
            SetupActionIcons();
            SetDescriptorActive(false);
            GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.DESELECT);
        }
        if (controls.Battle.Direction.triggered)
        {
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        }
        if (controls.Battle.Primary.triggered)
        {
            currentActiveUIElement.displayable.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
            string name = strings[0];

            switch(name)
            {
                case "Charge" :
                    Debug.Log("charge!");
                    
                    battleManager.currentActiveCharacter.characterBattlePhysics.Jump();
                    battleManager.currentActiveCharacter.characterData.statusEffects.Add(new CharacterStatusEffect(
                        2,1,battleManager.currentActiveCharacter.characterData.baseATK,CharacterStat.ATK,battleManager.currentActiveCharacter.characterData
                    ));
                    
                    BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.WAIT;
                    
                    battleManager.waitTimer = new Timer(1.5f);
                    battleManager.waitTimer.OnTimerEnd += battleManager.GetNextTurn;

                    
                    ClearCurrentSubMenu();
                    currentState = _playerTurnUIStates[6];
                    gameObject.SetActive(false);

                    break;
                case "Run Away" : 
                    Debug.Log("run away!");
                    break;
                case "Do Nothing" :
                    Debug.Log("do nothing!");

                    BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ANALYSIS;                    
                    ClearCurrentSubMenu();
                    currentState = _playerTurnUIStates[6];
                    gameObject.SetActive(false);

                    break;
            }
        }
    }
    #endregion
    #region Description Methods
    #endregion
    void SetDescriptorActive(bool active)
    {
        descriptionDisplay.transform.parent.gameObject.SetActive(active);
    }
    void SetDescriptionText(string text)
    {
        descriptionDisplay.text = text;
    }
}




using System.Collections.Generic;
using UnityEngine;
using BeriUtils;
using TMPro;

public class PlayerTurnUI : MonoBehaviour
{
    /*
        Class for managing the player's UI when it is currently their turn in battle. 
    */
    public Vector3[] listSpawnerOrigins = {new Vector3(1.5f,4.0f,0)}; // offsets for spawning list entries.
    public StaminaBar staminaBar;
    public UISelectable goButton;
    public List<UISelectable> playerActionIcons = new List<UISelectable>();
    public List<UISelectable> currentActiveUIElements = new List<UISelectable>();
    public UISelectable currentActiveUIElement;
    public UISelectable currentSelectedItem; // hack. probably a better way to do this. see itemselection()
    public UIListEntry listEntryPrefab;
    public UICursor cursor;
    public TextMeshPro playerActionIconDisplayText;
    public BattleManager battleManager;

    public List<BattleMove> playerMoveQueue = new List<BattleMove>();
    
    [SerializeField][Range(0.1f,2.0f)] float _uiElementMoveTimeLimit = 0.25f;
    [SerializeField] float _uiElementMovementTimer = 0f;
    [SerializeField][Range(0.1f,2.0f)] float _actionIconXScale = 1.25f;

    PrimaryControls controls;
    const float THETA = 2 * Mathf.PI;
    string _currentState;
    string[] _playerTurnUIStates = 
    {
        "ActionSelect",
        "Attack",
        "Items",
        "Tactics",
        "ItemUseSelection"
    };
    void Awake() 
    {
        controls = new PrimaryControls();
        controls.Enable();
        SetupActionIcons();
        _currentState = _playerTurnUIStates[0];
    }

    void Update()
    {
        if (CentralManager.GetStateManager() != null)
            battleManager = (BattleManager)CentralManager.GetStateManager();
        // start incrementing the ui movement couner.
        _uiElementMovementTimer += Time.deltaTime;
        if (_uiElementMovementTimer >= _uiElementMoveTimeLimit)
            _uiElementMovementTimer = _uiElementMoveTimeLimit;

        // switch statement for current ui state. 
        switch (_currentState)
        {
            case ("ActionSelect") :
            {
                ActionSelect();
                break;
            }
            case ("Attack") :
            case ("AttackTargetSelection") :
            {
                AttackSelection();
                break;
            }
            case ("Items") :
            case ("ItemUseSelection") :
            {
                ItemSelection();
                break;
            }
            default :
            {
                
                break;
            }
        }
        if (cursor.enabled)
            cursor.cursorSelectable = currentActiveUIElement;
    }
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
            float yOffset = transform.localScale.y;
            for (int i = 0; i < ListEntries.Count; i++)
            {
                Vector3 target = spawnerOrigin;
                target.y -= (i * yOffset);
                UIListEntry objEntry = Instantiate(listEntryPrefab);
                objEntry.selectable.InitializeValues(i,ListEntries.Count,true,false,true);
                objEntry.selectable.InitializePositions(spawnerOrigin,target);
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
        for (int i = 0; i < currentActiveUIElements.Count; i++)
        {
            if (currentActiveUIElements[i].isDestroyable)
                Destroy(currentActiveUIElements[i].gameObject);
            if (currentActiveUIElements[i].canBeDisabled)
                currentActiveUIElements[i].gameObject.SetActive(false);
        }
        currentActiveUIElements.Clear();
        _uiElementMovementTimer = 0;
        if (playerActionIconDisplayText.IsActive())
            playerActionIconDisplayText.gameObject.SetActive(false); // stop displaying the text under the active icon
        if (staminaBar.gameObject.activeSelf)
            { staminaBar.gameObject.SetActive(false);  staminaBar.currentLerpTime = 0;}
    }
    void Navigate(int shift) // Navigate through the current list of active UI elements. 
    {
        int i;
        for (i = 0; i < currentActiveUIElements.Count; i++)
        {
            currentActiveUIElements[i].ShiftCycle(shift);
            if (currentActiveUIElements[i].cycle == 0) { currentActiveUIElement = currentActiveUIElements[i]; }
        }
    }
    
#region ActionSelect Methods
    void ActionSelect() // Method for moving the actionIcons.
    {
        playerActionIconDisplayText.text = currentActiveUIElement.gameObject.name;
        currentActiveUIElements = new List<UISelectable>(playerActionIcons);
        if (cursor.gameObject.activeSelf)
            DisableCursor();

        if (controls.Battle.Direction.triggered)
        {
            _uiElementMovementTimer = 0;
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().x);
        }
        for (int i = 0; i < playerActionIcons.Count; i++)
            SmoothMoveActionIcon(i);
        
        if (controls.Battle.Primary.triggered)
        {
            switch (currentActiveUIElement.name)
            {
                case "Attack" :
                {
                    ClearCurrentSubMenu();
                    _currentState = _playerTurnUIStates[1];
                    InstantiateList(new List<IDisplayable>(battleManager.playerMoves),listSpawnerOrigins[0] + transform.position);
                    for (int i = 0; i < currentActiveUIElements.Count; i++)
                    {
                        currentActiveUIElements[i].cyclableElements += 1;
                    }
                    currentActiveUIElements.Add(goButton);
                    goButton.InitializeValues(currentActiveUIElements.Count - 1,currentActiveUIElements.Count,false,false,true);
                    
                    break;
                }
                case "Items" :
                {

                    if (battleManager.playerItems.Count == 0)
                        break;
                    ClearCurrentSubMenu();
                    _currentState = _playerTurnUIStates[2];
                    InstantiateList(new List<IDisplayable>(battleManager.playerItems),listSpawnerOrigins[0] + transform.position);
                    break;
                }
                case "Tactics" :
                {
                    _currentState = _playerTurnUIStates[3];
                    Debug.Log("Get prank'd nerd");
                    break;
                }
            }
        }
    }
    void SetupActionIcons() // initialize action icons. 
    {
        int i;
        for (i = 0; i < playerActionIcons.Count; i++)
        {
            playerActionIcons[i].gameObject.SetActive(true);
            playerActionIcons[i].cycle = i;
            playerActionIcons[i].cyclableElements = playerActionIcons.Count;
        }
        _uiElementMovementTimer = 0;
        playerActionIconDisplayText.gameObject.SetActive(true);
        currentActiveUIElement = playerActionIcons[0];
    }

    void SmoothMoveActionIcon(int i) // smoothly move the player icon at index i to appropriate position. [explicitly for actionselection menu] 
    {
        UISelectable icon = playerActionIcons[i];

        icon.GetComponent<SpriteRenderer>().sortingOrder = icon.cycle == 0 ? 101 : 100 - icon.cycle;
        icon.GetComponent<SpriteRenderer>().color = icon.cycle == 0 ? Color.white : Color.grey;

        Vector3 targetPosition = icon.initialPosition;
        targetPosition.x = icon.initialPosition.x + _actionIconXScale * (Mathf.Sin((THETA * icon.cycle)/icon.cyclableElements));
        float percentage = _uiElementMovementTimer/_uiElementMoveTimeLimit;
        icon.transform.position = Vector2.Lerp(icon.transform.position,targetPosition,percentage);

    }
#endregion 
#region AttackMenu

    void AttackSelection()
    {
        staminaBar.staminaCount = battleManager.currentActiveCharacter.characterData.baseStamina;
        goButton.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(true);
        staminaBar.targetPosition = transform.position + new Vector3(-10,0,0);
        if (controls.Battle.Direction.triggered)
        {
            _uiElementMovementTimer = 0;
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        }
        switch(_currentState)
        {
            case ("Attack") :
            {
                if (controls.Battle.Primary.triggered)
                {
                    if (currentActiveUIElement.displayable != null && playerMoveQueue.Count < battleManager.currentActiveCharacter.characterData.curStamina)
                    {
                        playerMoveQueue.Add((BattleMove)currentActiveUIElement.displayable);
                    }
                }
                if (controls.Battle.Secondary.triggered)
                {
                    _currentState = _playerTurnUIStates[0];
                    ClearCurrentSubMenu();
                    SetupActionIcons();
                }
                break;
            }
        }
    }
#endregion
#region ItemSelection methods
    void ItemSelection()
    {
        if (controls.Battle.Direction.triggered)
        {
            _uiElementMovementTimer = 0;
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        }
        switch(_currentState)
        {
            case ("Items") : // Menu selections for selecting an item. 
            {
                if (controls.Battle.Primary.triggered) // if the player selected an item, setup the next menu.
                {
                    currentSelectedItem = currentActiveUIElement; // <-- probably better way to do this. since the active ui element serve two purposes, we can't remove the item after using it by getting the index of the currently active selectable. this is just to store the index. 
                    ClearCurrentSubMenu();
                    _currentState = _playerTurnUIStates[4];
                    for (int i = 0; i < battleManager.playerCharacters.Count; i++)
                    {
                        currentActiveUIElements.Add(battleManager.playerCharacters[i].characterSelectable);
                    }
                    currentActiveUIElement = currentActiveUIElements[0];
                }
                if (controls.Battle.Secondary.triggered)
                {
                    _currentState = _playerTurnUIStates[0];
                    ClearCurrentSubMenu();
                    SetupActionIcons();
                }
                break;
            }
            case ("ItemUseSelection") : // player selected an item to use.
            {
                if (controls.Battle.Primary.triggered)
                {
                    battleManager.playerItems.RemoveAt(currentSelectedItem.index);
                    ClearCurrentSubMenu();
                    _currentState = _playerTurnUIStates[0];
                    SetupActionIcons();
                    currentSelectedItem = null;
                }
                if (controls.Battle.Secondary.triggered)
                {
                    _currentState = _playerTurnUIStates[2];
                    ClearCurrentSubMenu();
                    InstantiateList(new List<IDisplayable>(battleManager.playerItems),listSpawnerOrigins[0] + transform.position);
                }
                break;
            }
        }
    }
#endregion
}

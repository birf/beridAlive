using System.Collections.Generic;
using UnityEngine;
using BeriUtils;
using TMPro;

public class PlayerTurnUI : MonoBehaviour
{
    /*
        Class for managing the player's UI when it is currently their turn in battle. 
    */
    public List<UISelectable> playerActionIcons = new List<UISelectable>();
    public List<UISelectable> currentActiveUIElements = new List<UISelectable>();
    public UISelectable currentActiveUIElement;
    public TextMeshPro playerActionIconDisplayText;
    
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
        "Tactics"
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
            default :
            {
                Test();
                break;
            }
        }
        
    }
    void Test()
    {
        if (controls.Battle.Primary.triggered)
        {
            _currentState = _playerTurnUIStates[0];
            SetupActionIcons();
        }
    }
    void InstantiateList(List<UISelectable> ListEntries) // create a list of selectable items from the input list
    {
        
    }
    void ClearCurrentSubMenu()
    {
        for (int i = 0; i < currentActiveUIElements.Count; i++)
            currentActiveUIElements[i].gameObject.SetActive(false);
        _uiElementMovementTimer = 0;
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
        currentActiveUIElements = playerActionIcons;
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
                    Debug.Log("Attack!");
                    _currentState = _playerTurnUIStates[1];
                    break;
                }
                case "Items" :
                {
                    Debug.Log("Items!");
                    _currentState = _playerTurnUIStates[2];
                    break;
                }
                case "Tactics" :
                {
                    Debug.Log("Tactics!");
                    _currentState = _playerTurnUIStates[3];
                    break;
                }
            }
            playerActionIconDisplayText.gameObject.SetActive(false); // stop displaying the text under the active icon
            ClearCurrentSubMenu();
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

        Vector3 targetPosition = icon.initialPosition;
        targetPosition.x = icon.initialPosition.x + _actionIconXScale * (Mathf.Sin((THETA * icon.cycle)/icon.cyclableElements));
        float percentage = _uiElementMovementTimer/_uiElementMoveTimeLimit;
        icon.transform.position = Vector2.Lerp(icon.transform.position,targetPosition,percentage);
    }
}
#endregion 

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
    public List<UISelectable> currentlyActiveUIElements = new List<UISelectable>();
    public UISelectable currentActiveUIElement;
    public TextMeshPro playerActionIconDisplayText;
    
    [SerializeField][Range(0.1f,2.0f)] float _uiElementMoveTimeLimit = 0.25f;
    [SerializeField] float _uiElementMovementTimer = 0f;
    [SerializeField][Range(0.1f,2.0f)] float _actionIconXScale = 1.25f;


    PrimaryControls controls;
    const float THETA = 2 * Mathf.PI;
    string _currentState;
    string[] _playerTurnUIState = 
    {
        "ActionSelect",
        "Attack"
    };
    void Awake() 
    {
        controls = new PrimaryControls();
        controls.Enable();
        SetupActionIcons();
        _currentState = _playerTurnUIState[0];
    }

    void Update()
    {
        _uiElementMovementTimer += Time.deltaTime;
        if (_uiElementMovementTimer >= _uiElementMoveTimeLimit)
            _uiElementMovementTimer = _uiElementMoveTimeLimit;

        switch (_currentState)
        {
            case ("ActionSelect") :
            {
                ActionSelect();
                break;
            }   
        }
    }
    void ActionSelect() // Method for moving the actionIcons.
    {
        playerActionIconDisplayText.text = currentActiveUIElement.gameObject.name;
        currentlyActiveUIElements = playerActionIcons;
        if (controls.Battle.Direction.triggered)
        {
            _uiElementMovementTimer = 0;
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().x);
        }
        for (int i = 0; i < playerActionIcons.Count; i++)
            SmoothMoveActionIcon(i);
        
    }
    void Navigate(int shift) // Navigate through the input list. 
    {
        int i;
        for (i = 0; i < currentlyActiveUIElements.Count; i++)
        {
            currentlyActiveUIElements[i].ShiftCycle(shift);
            if (currentlyActiveUIElements[i].cycle == 0) { currentActiveUIElement = currentlyActiveUIElements[i]; }
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
        currentActiveUIElement = playerActionIcons[0];
    }
    void SmoothMoveActionIcon(int i) // smoothly move the player icon at index i to appropriate position. 
    {
        UISelectable icon = playerActionIcons[i];

        icon.GetComponent<SpriteRenderer>().sortingOrder = icon.cycle == 0 ? 101 : 100 - icon.cycle;

        Vector3 targetPosition = icon.initialPosition;
        targetPosition.x = icon.initialPosition.x + _actionIconXScale * (Mathf.Sin((THETA * icon.cycle)/icon.cyclableElements));
        float percentage = _uiElementMovementTimer/_uiElementMoveTimeLimit;
        icon.transform.position = Vector2.Lerp(icon.transform.position,targetPosition,percentage);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UISelectable))]
[RequireComponent(typeof(BattlePhysicsInteraction))]
public class CharacterGameBattleEntity : CharacterGameEntity
{
    /*
        Class for holding character data read from a CharacterScriptable during battle.
        Use this to read character data and manipulate it on screen.
    */
    public Animator characterAnimator;
    public RuntimeAnimatorController characterAnimations; // <-- currently playing character animations. interacts with ^^
    public BattlePhysicsInteraction characterBattlePhysics; // <-- The script for basic physics interactions in battle. 
    public UISelectable characterSelectable; // <-- The selectable for this character. (if needed)
    public BattleManager entityBattleManager; // <-- reference to the battle manager. 
    public BoxCollider2D boxCol;
    
    void Awake()
    {
        characterSelectable.cursorTarget = characterData.CharType == CharacterBase.CharacterType.PLAYER ? new Vector3(2f, 1f, 0) : new Vector3(-2f, 1.0f, 0);
        characterSelectable.cursorTarget += transform.position;
        CharacterSetup();
    }

    // Initialize character data from the character scriptable. 
    public void CharacterSetup()
    {
        if (characterScriptable != null)
        {
            characterData = new CharacterBase(characterScriptable);
            characterAnimations = characterScriptable.charAnimations;
        }

        characterAnimator = GetComponent<Animator>();
        characterAnimator.runtimeAnimatorController = characterAnimations;

        // if the character is on the left side or the right, make sure the correct offset is used.
        characterSelectable = GetComponent<UISelectable>();
        characterSelectable.isDestroyable = false;

        switch (characterData.CharType)
        {
            case (CharacterBase.CharacterType.PLAYER):
                {
                    gameObject.tag = "Player";
                    gameObject.layer = 7;
                    break;
                }
            case (CharacterBase.CharacterType.ENEMY):
                {
                    gameObject.tag = "Enemy";
                    gameObject.layer = 8;
                    break;
                }
        }
        
        characterBattlePhysics = GetComponent<BattlePhysicsInteraction>();
        characterBattlePhysics.localGroundYCoordinate = transform.position.y;
    }
    // Check current context of the game, and change states accordingly.
    void Update()
    {

        if (CentralManager.GetStateManager() != null)
            entityBattleManager = (BattleManager)CentralManager.GetStateManager();
        characterSelectable.cursorTarget = characterData.CharType == CharacterBase.CharacterType.PLAYER ? new Vector3(2f, 1f, 0) : new Vector3(-2f, 1.0f, 0);
        characterSelectable.cursorTarget += transform.position;

        // i fucking hate unity lmao. for some reason, even though the collider object exists upon instantiation, setting this up in 
        // character setup is a null reference. if someone smarter than me can fix this that'd be swell. 
        
        if (GetComponent<BoxCollider2D>().size != characterScriptable.battleHitBoxSize)
            GetComponent<BoxCollider2D>().size = characterScriptable.battleHitBoxSize;
        
        
    }

    public void KillCharacterInBattle()
    {
        BattleManager b = (BattleManager)entityBattleManager;
        BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ANALYSIS;
        
        if (characterData.CharType == CharacterBase.CharacterType.ENEMY)
            b.enemyCharacters.Remove(this);
        else if (characterData.CharType == CharacterBase.CharacterType.PLAYER)
            b.playerCharacters.Remove(this);
        
        b.CharacterGameBattleEntities.Remove(this);
        Destroy(gameObject); 
    }
}
public enum BattleEntityState
{
    NORMAL,
    MOVING,
    DAMAGED,
    ATTACKING
}
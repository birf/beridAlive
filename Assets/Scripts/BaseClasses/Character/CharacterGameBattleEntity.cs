using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UISelectable))]
[RequireComponent(typeof(BattlePhysicsInteraction))]
public class CharacterGameBattleEntity : MonoBehaviour
{
    /*
        Class for holding character data read from a CharacterScriptable during battle.
        Use this to read character data and manipulate it on screen.
    */
    public Animator characterAnimator;
    public RuntimeAnimatorController characterAnimations; // <-- currently playing character animations. interacts with ^^
    public CharacterBase characterData; //  <-- Base character data for this entity. Includes health, atk, etc.
    public CharacterScriptable characterScriptable; // <-- Whenever default or saved values from character scriptable are needed, use this.
    public BattlePhysicsInteraction characterBattlePhysics; // <-- The script for basic physics interactions in battle. 
    public UISelectable characterSelectable; // <-- The selectable for this character. (if needed)
    public BattleManager entityBattleManager; // <-- reference to the battle manager. 
    
    void Awake()
    {
        CharacterSetup();
        characterAnimator.Play("battle_idle"); // <-- tester

        characterSelectable.cursorTarget = characterData.CharType == CharacterBase.CharacterType.PLAYER ? new Vector3(2f, 1f, 0) : new Vector3(-2f, 1.0f, 0);
        characterSelectable.cursorTarget += transform.position;
    }

    // Initialize character data from the character scriptable. 
    void CharacterSetup()
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

    }
    // Check current context of the game, and change states accordingly.
    void Update()
    {
        if (CentralManager.GetStateManager() != null)
            entityBattleManager = (BattleManager)CentralManager.GetStateManager();
        characterSelectable.cursorTarget = characterData.CharType == CharacterBase.CharacterType.PLAYER ? new Vector3(2f, 1f, 0) : new Vector3(-2f, 1.0f, 0);
        characterSelectable.cursorTarget += transform.position;
        
    }

    public void KillCharacterInBattle()
    {
        BattleManager b = (BattleManager)entityBattleManager;
        BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ANALYSIS;
        
        if (characterData.CharType == CharacterBase.CharacterType.ENEMY)
            b.enemyCharacters.Remove(this);
        else if (characterData.CharType == CharacterBase.CharacterType.PLAYER)
            b.playerCharacters.Remove(this);
        
        b.CharacterGameObjects.Remove(this);
        Destroy(gameObject); // <-- for now. tester
    }
}

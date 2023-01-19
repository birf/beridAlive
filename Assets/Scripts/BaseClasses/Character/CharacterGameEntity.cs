using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UISelectable))]

public class CharacterGameEntity : MonoBehaviour
{
    /*
        Class for holding character data read from a CharacterScriptable.
        Use this to read character data and manipulate it on screen regardless of context.
    */
    public Animator characterAnimator;
    public RuntimeAnimatorController characterAnimations; // <-- currently playing character animations. interacts with ^^
    public CharacterBase characterData; //  <-- Use this when communicating with other gameObjects.
    public CharacterScriptable characterScriptable; // <-- Whenever default or saved values from character scriptable are needed, use this.
    public BattlePhysicsInteraction characterBattlePhysics; // <-- The script for basic physics interactions in battle. 
    public UISelectable characterSelectable; // <-- The selectable for this character.
    public GameManager currentManager;
    void Awake()
    {
        CharacterSetup();   
        GetCorrectContext();

        characterSelectable.cursorTarget = characterData.CharType == CharacterBase.CharacterType.PLAYER ? new Vector3(2f,1f,0) : new Vector3(-2f,1.0f,0);
        characterSelectable.cursorTarget += transform.position;
    }

    // Initialize character data from the character scriptable. 
    void CharacterSetup()
    {
        characterData = new CharacterBase(characterScriptable);
        
        characterAnimator = GetComponent<Animator>();
        characterAnimations = characterScriptable.charAnimations;
        characterAnimator.runtimeAnimatorController = characterAnimations;

        // if the character is on the left side or the right, make sure the correct offset is used.
        characterSelectable = GetComponent<UISelectable>();
        characterSelectable.isDestroyable = false;

        

        switch(characterData.CharType)
        {
            case (CharacterBase.CharacterType.PLAYER) :
            {
                gameObject.tag = "Player";
                gameObject.layer = 7;
                break;
            }
            case(CharacterBase.CharacterType.ENEMY) :
            {
                gameObject.tag = "Enemy";
                gameObject.layer = 8;

                break;
            }
        }

    }
    // Check current context of the game, and change states accordingly.
    void GetCorrectContext()
    {
        switch (CentralManager.CurrentContext)
        {
            // testing purposes. just to play test animations.
            case (CentralManager.Context.BATTLE) :
            {
                // testing
                BattleManager test = (BattleManager)currentManager; // reference the current battle manager under the appropriate type.
                characterAnimator.Play("battle_idle");
                break;
            }
            // this would be where to place behaviour for other contexts / states
        }
    }
    void Update()
    {
        if (CentralManager.GetStateManager() != null)
            currentManager = CentralManager.GetStateManager();

        GetCorrectContext();
    }
    public void UpdateStat(string statistic, int modifier)
    {
        switch(statistic) 
        {
            case("Health") :
            {
                characterData.curHP += modifier;
                if (characterData.curHP <= 0)
                    characterData.curHP = 0;
                if (characterData.curHP > characterData.baseHP)
                    characterData.curHP = characterData.baseHP;
                break;
            }
        }
    }
}

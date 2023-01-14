using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
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
    public GameManager currentManager;
    void Awake()
    {
        CharacterSetup();   
        if (CentralManager.GetStateManager() != null)
            currentManager = CentralManager.GetStateManager();
        GetCorrectContext();
        
    }

    // Initialize character data from the character scriptable. 
    void CharacterSetup()
    {
        characterData = new CharacterBase(characterScriptable);
        
        characterAnimator = GetComponent<Animator>();
        characterAnimations = characterScriptable.charAnimations;
        characterAnimator.runtimeAnimatorController = characterAnimations;

    }
    // Check current context of the game, and change states accordingly.
    void GetCorrectContext()
    {
        switch (CentralManager.CurrentContext)
        {
            case (CentralManager.Context.BATTLE) :
            {
                // testing
                BattleManager test = (BattleManager)currentManager; // reference the current battle manager under the appropriate type.
                characterAnimator.Play("battle_idle");
                break;
            }
            case (CentralManager.Context.OVERWORLD) :
            {
                Debug.Log("some other thing");
                break;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGameOverworldEntity : CharacterGameEntity
{
    public OverworldManager overworldManager;
    public List<CharacterScriptable> partnerCharacters = new List<CharacterScriptable>();

    void Awake()
    {
        if (characterScriptable)
        {
            characterData = new CharacterBase(characterScriptable);
        }        
        switch (characterData.CharType)
        {
            case (CharacterBase.CharacterType.DEFAULT) :
                gameObject.tag = "Default";
                break;
            case (CharacterBase.CharacterType.PLAYER) :
                gameObject.tag = "Player";
                gameObject.layer = 7;
                break;
            case (CharacterBase.CharacterType.ENEMY) :
                gameObject.tag = "Enemy";
                gameObject.layer = 8;
                break;
        }
    }
    void Update()
    {
        if (!overworldManager)
            overworldManager = (OverworldManager)CentralManager.GetStateManager();
        
    }
}

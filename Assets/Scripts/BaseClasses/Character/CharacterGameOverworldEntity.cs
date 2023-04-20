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
        if (characterData.CharType == CharacterBase.CharacterType.PLAYER)
        {
            gameObject.tag = "Player";
            gameObject.layer = 7;

        }
        else
        {
            gameObject.tag = "Enemy";
            gameObject.layer = 8;
        }    

    }
    void Update()
    {
        if (!overworldManager)
            overworldManager = (OverworldManager)CentralManager.GetStateManager();     
           
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable/Character Data")]
[System.Serializable]
public class CharacterScriptable : ScriptableObject , IDisplayable
{
    /*
        A scriptable object that is used to easily store character data for use in character classes.
        When creating a new character, start with it's stats, animations, etc. in here, and add this to 
        each instance of a character in the scene as appropriate.
    */
    public Sprite[] characterSprites;
    public CharacterBase characterData;
    public RuntimeAnimatorController charAnimations;

    public List<BattleMove> moves = new List<BattleMove>();
    // public List<ItemData> items = new List<ItemData>();
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = characterSprites;
        ints = new int[] {characterData.baseHP, characterData.baseAtk, characterData.baseDef, characterData.baseStamina};
        strings = new string[] {characterData.CharacterName, characterData.CharacterDescription};
    }
}

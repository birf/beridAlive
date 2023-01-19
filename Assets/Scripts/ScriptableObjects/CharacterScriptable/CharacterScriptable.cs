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
    public Vector2 battleHitBoxSize;
    public List<BattleMove> characterMoves = new List<BattleMove>();
    public List<ItemData> characterItems = new List<ItemData>();
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = characterSprites;
        ints = new int[] {characterData.baseHP, characterData.baseATK, characterData.baseDEF, characterData.baseSTAMINA};
        strings = new string[] {characterData.CharacterName, characterData.CharacterDescription};
    }
}

using System.Collections.Generic;
[System.Serializable]
public class CharacterBase
{
    /*
        Base class for characters, which includes player, enemies, and NPCs.
        Only includes character stats. Flavor, such as animations, sprite data, descriptions, etc. are available in the CharacterData class.
        These numbers are the ones that can be manipulated within the battle, and therefore must be separate from the character scriptable. 

        IMPORTANT : Character game entities should get an instance of this class.

    */
    public enum CharacterType
    {
        DEFAULT,
        PLAYER,
        ENEMY
    }
    public CharacterType CharType;
    public int curHp;
    public int baseHP;
    public int curAtk;
    public int baseAtk;
    public int curDef;
    public int baseDef;
    public int curStamina;
    public int baseStamina;
    public string CharacterName;
    public string CharacterDescription;

    ///<summary>
    ///Create a new character class. (Should rarely be called.)
    ///</summary>

    public CharacterBase(string name, int baseHP, int baseDef, int baseStamina)
    {
        CharacterName = name;
        this.baseHP = baseHP;
        this.baseDef = baseDef;
        this.baseStamina = baseStamina;
        CharType = CharacterType.DEFAULT;

        this.curHp = baseHP;
        this.curAtk = baseAtk;
        this.curDef = baseDef;
        this.curStamina = baseStamina;
    }
    public CharacterBase(CharacterScriptable input)
    {
        CharacterBase inputData = input.characterData;
        CharacterName = inputData.CharacterName;
        this.baseAtk = inputData.baseAtk;
        this.baseDef = inputData.baseDef;
        this.baseHP = inputData.baseHP;
        this.baseStamina = inputData.baseHP;
        CharType = inputData.CharType;
    }
}

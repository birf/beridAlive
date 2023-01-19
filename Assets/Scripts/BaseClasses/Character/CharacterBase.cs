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
    public int curHP;
    public int baseHP;
    public int curATK;
    public int baseATK;
    public int curDEF;
    public int baseDEF;
    public int curSPEED;
    public int baseSPEED;
    public int curSTAMINA;
    public int baseSTAMINA;
    public string CharacterName;
    public string CharacterDescription;

    ///<summary>
    ///Create a new character class. (Should rarely be called.)
    ///</summary>

    public CharacterBase(string name, int baseHP, int baseDef, int baseStamina)
    {
        CharacterName = name;
        this.baseHP = baseHP;
        this.baseDEF = baseDef;
        this.baseSTAMINA = baseStamina;
        CharType = CharacterType.DEFAULT;

        this.curHP = baseHP;
        this.curATK = baseATK;
        this.curDEF = baseDef;
        this.curSTAMINA = baseStamina;
    }
    public CharacterBase(CharacterScriptable input)
    {
        CharacterBase inputData = input.characterData;
        CharacterName = inputData.CharacterName;
        CharacterDescription = inputData.CharacterDescription;
        this.baseHP = inputData.baseHP;
        this.baseATK = inputData.baseATK;
        this.baseDEF = inputData.baseDEF;
        this.baseSPEED = inputData.baseSPEED;
        this.baseSTAMINA = inputData.baseSTAMINA;
        
        this.curHP = inputData.curHP;
        this.curATK = inputData.curATK;
        this.curDEF = inputData.curDEF;
        this.curSTAMINA = inputData.curSTAMINA;
        this.curSPEED = inputData.curSPEED;

        CharType = inputData.CharType;
    }
}

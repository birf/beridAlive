using System;
using System.Collections.Generic;
[System.Serializable]
public class CharacterBase
{
    /*
        Base class for characters, which includes player, enemies, and NPCs.
        Only includes character stats. Flavor, such as animations, sprite data, descriptions, etc. are available in the CharacterScriptable class.
        These stats are what can be manipulated in battle.
        IMPORTANT : Character game entities should get an instance of this class from a character scriptable.

    */
    public enum CharacterType
    {
        DEFAULT,
        PLAYER,
        ENEMY
    }
    public List<CharacterStatusEffect> statusEffects = new List<CharacterStatusEffect>();
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
    ///<summary>
    ///Create a new character class from character scriptable.
    ///</summary>
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
    ///<summary>
    ///Update the input statistic for this character by the input float.
    ///</summary>
    public void AddToStat(CharacterStats statistic, float input, bool ignoreBaseStat)
    {
        switch(statistic) 
        {
            case(CharacterStats.HP) :
            {
                // if the input is negative, assume that this is an attack, add it to curHP - curDEF. else, just add.
                curHP += input < 0 ? ((int) Math.Floor(input)) + curDEF : (int)Math.Floor(input); 
                
                if (!ignoreBaseStat)
                {   
                    if (curHP <= 0)
                        curHP = 0;
                    if (curHP > baseHP)
                        curHP = baseHP;
                }
                
                break;
            }
            case(CharacterStats.STAMINA) :
            {
                curSTAMINA += (int)input; 
                if (curSTAMINA >= baseSTAMINA && !ignoreBaseStat)
                    curSTAMINA = baseSTAMINA;
                break;
            }
            case(CharacterStats.ATK) :
            {
                curATK += (int) input;
                if (curATK >= baseATK && !ignoreBaseStat)
                    curATK = baseATK;
                break;
            }
        }
    }
    public void SetCurrentStat(CharacterStats stat, float input)
    {
        switch(stat) 
        {
            case(CharacterStats.ATK) :
                curATK = (int) input;  
                break; 
            case(CharacterStats.DEF) :
                curDEF = (int) input;  
                break;            
            case(CharacterStats.HP) :
                curHP = (int) input;  
                break;
            case(CharacterStats.SPEED) :
                curSPEED = (int) input;  
                break;
            case(CharacterStats.STAMINA) :
                curSTAMINA = (int) input;  
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatusEffect 
{
    public int duration = 0;
    public int difference = 0;
    public int basePotency = 0;
    public CharacterStat statAffected;
    CharacterBase activeCharacter;


    public CharacterStatusEffect(int duration, int difference, int basePotency, CharacterStat stat, CharacterBase characterAffected)
    {
        this.duration = duration;
        this.difference = difference;
        this.statAffected = stat;
        this.basePotency = basePotency;
        activeCharacter = characterAffected;

        characterAffected.AddToStat(stat, (int) difference, true);
    }
    public void RevertChanges(bool ignoreBaseStat)
    {
        activeCharacter.AddToStat(statAffected, -difference, ignoreBaseStat);
    }
}

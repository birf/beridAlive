using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatusEffect 
{
    public int duration = 0;
    public int difference = 0;
    public int basePotency = 0;
    public CharacterStats statAffected;
    CharacterBase activeCharacter;


    public CharacterStatusEffect(int duration, int difference, int basePotency, CharacterStats stat, CharacterBase characterAffected)
    {
        this.duration = duration;
        this.difference = difference;
        this.statAffected = stat;
        this.basePotency = basePotency;
        activeCharacter = characterAffected;
        ChangeStat(true);
    }
    public void ChangeStat(bool ignoreBaseStat)
    {
        activeCharacter.AddToStat(statAffected, difference, ignoreBaseStat);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

[CreateAssetMenu(fileName = "StatsData", menuName = "Scriptable/Stats Data")]
public class Stats : ScriptableObject, IDisplayable 
{

    /*
        Scriptable Object just for displaying tactics options to the player.
    */
    public Sprite mainSprite;
    public CharacterStat statAffected;

    public string statName;
    public string statDescription;
    public int potency;
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        ints = new int[] {potency};
        sprites = new Sprite[] {mainSprite};
        strings = new string[] {statName, statDescription};
    }

}

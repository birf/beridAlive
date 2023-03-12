using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TacticsData", menuName = "Scriptable/Tactics Data")]
public class Tactics : ScriptableObject, IDisplayable 
{

    /*
        Scriptable Object just for displaying tactics options to the player.
    */
    public string tacticName;
    public string tacticDescription;
    public Sprite mainSprite;
    public int potency;
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        ints = new int[] {potency};
        sprites = new Sprite[] {mainSprite};
        strings = new string[] {tacticName, tacticDescription};
    }

}

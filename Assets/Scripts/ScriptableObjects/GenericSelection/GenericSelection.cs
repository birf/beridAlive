using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

[CreateAssetMenu(fileName = "GenericSelection", menuName = "Scriptable/Generic Selection")]
public class GenericSelection : ScriptableObject, IDisplayable 
{

    /*
        Scriptable Object just for displaying tactics options to the player.
    */
    public Sprite mainSprite;
    public string selectionName;
    public string selectionDescription;
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        ints = new int[] {-1};
        sprites = new Sprite[] {mainSprite};
        strings = new string[] {selectionName, selectionDescription};
    }

}
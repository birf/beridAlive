using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BattleMove", menuName = "Scriptable/Battle Move")]
public class BattleMove : ScriptableObject, IDisplayable
{
    /*
        Information about a move used by a character in battle. For player characters, these will be intended to 
        string into each other for combos. For enemies, these will be one-and-dones.
    */

    ///<summary>
    ///The main GameObject that is to be spawned that will be responsible for all behavior associated with this move. 
    ///</summary>
    public GameObject mainMoveSpawn;
    ///<summary>
    ///The launch velocity that this move should send the opponent. (If applicable.)
    ///</summary>
    public Vector3 launchVelocity;
    public RuntimeAnimatorController animations; // <-- to be removed. to circumvent the issue of generality, every enemy gets a fixed, maximum amount of moves.
    public Sprite moveSpriteSmall;
    public int damage;
    public int staminaCost;
    public string moveName;
    public string moveDescription;
    
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = new Sprite[]{moveSpriteSmall};
        ints = new int[]{staminaCost,damage,-1};
        strings = new string[]{moveName,moveDescription};
    }
}

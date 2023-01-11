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
    public Vector3 moveVelocity;
    ///<summary>
    ///The animations used for this move.
    ///</summary>
    public RuntimeAnimatorController animations;
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

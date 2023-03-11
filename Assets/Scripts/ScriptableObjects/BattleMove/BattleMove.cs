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
    public GameObject mainMoveGameObject;
    ///<summary>
    ///The launch velocity that this move should primarily send the opponent. (If applicable.)
    ///</summary>
    public Vector3 mainLaunchVelocity;
    ///<summary>
    /// Animations specific to this move. While not needed in every move, it may be more convenient to interface with this set
    /// of animations instead of a large web of unrelated animations in Beri's main animation controller, for example. 
    ///</summary>
    public RuntimeAnimatorController moveSpecificAnimations;
    public Sprite moveSpriteSmall;
    public int damage;
    public int staminaCost;
    public string moveName;
    public string moveDescription;
    public bool mustBeNearEnemy;

    public void SetupMainMoveGameObject(CharacterGameBattleEntity targetEnemy, BattleMove parentMove, BattleManager battleManager)
    {
        mainMoveGameObject.GetComponent<ATKScript>().targetEnemy = targetEnemy;
        mainMoveGameObject.GetComponent<ATKScript>().parentMove = parentMove;
        mainMoveGameObject.GetComponent<ATKScript>().battleManager = battleManager;
    }    
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = new Sprite[]{moveSpriteSmall};
        ints = new int[]{staminaCost,damage,-1};
        strings = new string[]{moveName,moveDescription};
    }
}

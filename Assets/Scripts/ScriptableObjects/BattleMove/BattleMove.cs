using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

public enum MoveType
{
    NONE,
    GRAB,
    PUNCH,
    PROJECTILE
}

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
    public RuntimeAnimatorController defaultAnimations;
    public Sprite moveSpriteSmall;
    public MoveType moveType;
    public float cooldownTime;
    public int damage;
    public int staminaCost;
    public string moveName;
    public string moveDescription;
    public string moveHelp;
    public bool isFinisher;

    public void SetupMainMoveGameObject(CharacterGameBattleEntity targetEnemy, BattleMove parentMove, BattleManager battleManager)
    {
        mainMoveGameObject.GetComponent<ATKScript>().targetEnemy = targetEnemy;
        mainMoveGameObject.GetComponent<ATKScript>().parentMove = parentMove;
        mainMoveGameObject.GetComponent<ATKScript>().battleManager = battleManager;
        mainMoveGameObject.GetComponent<ATKScript>().caster = battleManager.currentActiveCharacter;
        
        defaultAnimations = battleManager.currentActiveCharacter.GetComponent<Animator>().runtimeAnimatorController;
        
    }   
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = new Sprite[]{moveSpriteSmall};

        // ints[2], don't display number
        // ints[3], -1 -> move is finisher, 0 -> move isn't finisher
        ints = new int[]{staminaCost,damage,-1, 0};
        if (isFinisher)
            ints[3] = -1;

        strings = new string[]{moveName,moveDescription, moveHelp};
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectable : MonoBehaviour
{
    /*
        Class for representing what can be selected in the UI.
        Attach to any object that necessitates you 'selecting' that object via a GUI.
    */

    public IDisplayable displayable;
    public Vector3 cursorTarget; // position in space where a cursor object should move to.
    public Vector3 initialPosition; // initial position of this selectable. useful for icons that move.
    public Vector3 targetPosition; // target position of this selectable (if applicable).
    public int cycle = 0; // what cycle this selectable is currently on. 
    public int cyclableElements = 0; // how many elements should this selectable be concerned with. 
                                     // i.e., if there is a list of 4 items, there are 4 to keep track of. 
    public bool isDestroyable = true; // can we destroy this object?
    public bool cursorIsHovering = false; // is the cursor (if one is active) hovering over this object?
    public bool canBeDisabled = true; // can this object be disabled?
    public bool bobbable = false;


    void Awake()
    {
        if (initialPosition == Vector3.zero)
            initialPosition = transform.position;
    }
    void OnDisable()
    {
        transform.position = initialPosition == null ? transform.position : initialPosition;
    }
    ///<summary>
    ///Set up the integer and boolean values for this selectable.
    ///</summary>
    public void InitializeValues(int initCycle, int divisions, bool destroyable, bool hovering, bool disable)
    {
        cycle = initCycle;
        cyclableElements = divisions;
        isDestroyable = destroyable;
        cursorIsHovering = hovering;
        canBeDisabled = disable;
    }
    ///<summary>
    ///Set up the initial and target positions for this selectable.
    ///</summary>
    public void InitializePositions(Vector3 initial, Vector3 target)
    {
        initialPosition = initial;
        targetPosition = target;
    }
    public void ShiftCycle(int shift) // move up or down this ui element in the cycle. 
    {
        if (shift < 0)
            shift = cyclableElements - 1;
        cycle = (cycle + shift) % cyclableElements;
        if (cycle < 0)
            cycle += cyclableElements;
    }
}

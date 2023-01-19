using UnityEngine;
using TMPro;

public class UIListEntry : MonoBehaviour
{
    /*
        Class for displaying IDisplayable objects via a selectable list entry. 
    */
    public UISelectable selectable; // UISelectable component for this object.
    public SpriteRenderer displaySprite;
    public SpriteRenderer background;
    public TextMeshPro displayText;
    public TextMeshPro displayNumber;
    public Vector3 offset = new Vector3(-3.5f,0,0);
    public Vector3 targetPosition; 
    [Range(0.1f,50.0f)]public float lerpSpeed = 1.0f;

    void Awake()
    {
        displayText.sortingOrder = 100 - selectable.cycle; // -----\
        displayNumber.sortingOrder = 100 - selectable.cycle; // ---->-- ensure that every element is sorted correctly when created.
        displaySprite.sortingOrder = 100 - selectable.cycle; // ---/
        background.sortingOrder = 50 - selectable.cycle; //------/
    }
    void Update()
    {
        selectable.cursorTarget = transform.position + offset;
        Move();
    }
        
    void Move() // move this to the correct spot dictated by the offset. 
    {
        transform.position = Vector3.Lerp(transform.position,selectable.targetPosition, Time.deltaTime * lerpSpeed);
    }
    public void SetDisplayData(IDisplayable data) // set the correct displayable data for each item in the entry. call this first before
                                                  // instantiating the listentry object. 
    {
        data.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
        if (sprites.Length != 0)
            displaySprite.sprite = sprites[0];
        if (strings.Length != 0)
            displayText.text = strings[0];
        if (ints.Length == 0 || ints[0] == -1)
            displayNumber.text = "";
        else
            displayNumber.text = "" + ints[0];
    }
}

using UnityEngine;
using TMPro;

public class ListEntry : MonoBehaviour
{
    /*
        Class for displaying IDisplayable objects via a selectable list entry. 
    */
    public UISelectable selectable;
    public SpriteRenderer displaySprite;
    public TextMeshPro displayText;
    public TextMeshPro displayNumber;
    public Vector3 offset = new Vector3(-3.0f,0,0);
    public Vector3 targetPosition;
    public float lerpSpeed = 1.0f;
    float _currentLerpTime = 0;

    void Update()
    {
        selectable.cursorTarget = transform.position + offset;   
        Move();
    }
    void Move()
    {
        _currentLerpTime += Time.deltaTime;
        if (_currentLerpTime > lerpSpeed)
            _currentLerpTime = 0;
        float percentage = _currentLerpTime/lerpSpeed;
        transform.position = Vector3.Lerp(transform.position,targetPosition,percentage);
    }
    public void SetDisplayData(IDisplayable data)
    {
        data.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
        displaySprite.sprite = sprites[0];
        displayText.text = strings[0];
        if (ints[0] == -1)
            displayNumber.text = "";
        else
            displayNumber.text = "" + ints[0];
    }
}

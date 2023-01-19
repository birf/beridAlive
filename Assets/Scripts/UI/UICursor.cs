using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICursor : MonoBehaviour
{
    /*
        Script simply to follow whichever is the current active UI element.
    */
    public UISelectable cursorSelectable;
    public Vector3 normalScale;
    public Vector3 flippedScale;
    [SerializeField][Range(0.1f,50.0f)] float lerpSpeed = 0.25f;
    
    Vector3 _targetPositionLastFrame;
    float _currentLerpTime;


    public void SetCursorInitialPosition(Vector3 position)
    {
        transform.position = position;
    }
    
    void Awake()
    {
        normalScale = transform.localScale;
        flippedScale = new Vector3(-normalScale.x,normalScale.y,normalScale.z);
        
    }

    void Update()
    {
        if (cursorSelectable != null)
        {
            Move();
            if (cursorSelectable.flipCursor)
                transform.localScale = flippedScale;
            else
                transform.localScale = normalScale;
        }
    }

    void Move() // move this to the correct spot dictated by the offset. 
    {
        transform.position = Vector3.Lerp(transform.position,cursorSelectable.cursorTarget,Time.deltaTime * lerpSpeed);
        _targetPositionLastFrame = cursorSelectable.cursorTarget;
    }

}

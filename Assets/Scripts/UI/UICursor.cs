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
    [SerializeField][Range(0.1f,2.0f)] float lerpSpeed = 0.25f;
    
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
        if (_targetPositionLastFrame != cursorSelectable.cursorTarget)
            _currentLerpTime = 0;
        _currentLerpTime += Time.deltaTime;
        if (_currentLerpTime > lerpSpeed)
            _currentLerpTime = 0;
        float percentage = _currentLerpTime/lerpSpeed;
        transform.position = Vector3.Lerp(transform.position,cursorSelectable.cursorTarget,percentage);
        _targetPositionLastFrame = cursorSelectable.cursorTarget;
    }

}

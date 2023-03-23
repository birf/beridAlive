using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterGameBattleEntity))]
public class BattleDropShadow : MonoBehaviour
{
    [SerializeField] BattlePhysicsInteraction _characterPhysics;
    [SerializeField] Transform _dropShadow;
    [SerializeField] [Range(0.5f,3.0f)] float _dropDistance = 2.25f;
    
    public Vector2 dropShadowSize;

    float _localYLastFrame = 0f;
    float _internalYCoordinate;

    void Awake()
    {
        _internalYCoordinate = _dropShadow.position.y - _dropDistance;
        _dropShadow.transform.localScale = dropShadowSize;
    }
    void Update()
    {
        Vector3 _internalPosition = _dropShadow.position;
        _internalPosition.y = _internalYCoordinate;

        if (_localYLastFrame != _characterPhysics.localGroundYCoordinate - _dropDistance)
            _internalYCoordinate = _characterPhysics.localGroundYCoordinate - _dropDistance;

        _dropShadow.position = _internalPosition;
        _localYLastFrame = _characterPhysics.localGroundYCoordinate;
    }
    public void SetDropShadowSize(Vector2 size)
    {
        dropShadowSize = size;
        _dropShadow.transform.localScale = size;
    }
}

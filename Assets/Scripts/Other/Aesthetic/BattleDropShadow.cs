using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDropShadow : MonoBehaviour
{
    [SerializeField] BattlePhysicsInteraction _characterPhysics;
    [SerializeField] Transform _dropShadow;

    float _localYLastFrame = 0f;
    float _internalYCoordinate;

    void Awake()
    {
        _internalYCoordinate = _dropShadow.position.y - 2.25f;
    }
    void Update()
    {
        Vector3 _internalPosition = _dropShadow.position;
        _internalPosition.y = _internalYCoordinate;

        if (_localYLastFrame != _characterPhysics.localGroundYCoordinate -2.25f)
            _internalYCoordinate = _characterPhysics.localGroundYCoordinate - 2.25f;

        _dropShadow.position = _internalPosition;
        _localYLastFrame = _characterPhysics.localGroundYCoordinate;
    }
}

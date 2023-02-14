using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

[RequireComponent(typeof(BoxCollider2D))]
public class OverworldPlayerMove : MonoBehaviour
{
    [SerializeField][Range(0.1f,25.0f)] float _movementSpeed = 5.0f;
    [SerializeField] LayerMask _validLayers;
    [SerializeField] Camera _playerCamera;
    BoxCollider2D _boxCol;
    PrimaryControls controls;
    Vector2 _internalPosition;
    Vector2 _internalVelocity;
    Collider2D[] _colliderBuffer = new Collider2D[3];
    

    // Start is called before the first frame update
    void Awake()
    {
        controls = new PrimaryControls();
        controls.Enable();
        _boxCol = GetComponent<BoxCollider2D>();
    }

    void OnDisable() 
    {
        controls.Disable();
    }
    void OnEnable()
    {
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        GetState();
        //
        _internalPosition += _internalVelocity;
        //
        CheckCollisions();
        SetState();
    }
    void GetState()
    {
        _internalPosition = transform.position;
        _internalVelocity = controls.Overworld.Move.ReadValue<Vector2>() * _movementSpeed * Time.deltaTime;
    }
    void CheckCollisions()
    {
        
        if (Physics2D.OverlapBoxNonAlloc(transform.position,_boxCol.size,0f,_colliderBuffer,_validLayers) > 0)
        {
            OverworldManager om = (OverworldManager)CentralManager.GetStateManager();
            for (int i = 0; i < _colliderBuffer.Length; i++)
            {
                if (_colliderBuffer[i])
                {
                    om.currentEnemyEncounter = _colliderBuffer[i].GetComponent<CharacterGameOverworldEntity>();
                }
            }
            om.BattleStart();
        }
    }
    void SetState()
    {
        transform.position = _internalPosition;
    }
}

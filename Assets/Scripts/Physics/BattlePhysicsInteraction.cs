using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils;

[RequireComponent(typeof(BoxCollider2D))]
public class BattlePhysicsInteraction : MonoBehaviour
{

    /*
        Script in charge for launching and bouncing character entities in battle. 
    */

#region Primitives
    public bool isGrounded = true;
    public bool isHit = false;
    public float moveSpeed = 5.0f;
    int _groundBounces = 5;
    float _gravity = 9.81f;
    float _lerpTime;
    const int MAXGROUNDBOUNCES = 5;
    const float MINMOVEDISTANCE = 0.01f;
#endregion

#region Classes and Structs
    public Vector2 startPosition = new Vector2();
    Vector2 _internalPosition = new Vector2();    
    Vector2 _internalVelocity = new Vector2();
    RaycastHit2D[] _hitBuffer = new RaycastHit2D[5];
    Collider2D[] _collisionBuffer = new Collider2D[3];
    [SerializeField] BoxCollider2D _boxCol;
    [SerializeField] LayerMask _validHits;
    [SerializeField][Range(0f,5.0f)] float _gravityScale = 2.0f;

#endregion

#region Private Fields

#endregion
    void Awake()
    {
        _boxCol = GetComponent<BoxCollider2D>();
        _boxCol.size = GetComponent<CharacterGameEntity>().characterScriptable.battleHitBoxSize;
        isGrounded = true;
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        GetState();   
        if (!isGrounded)
        {
            UpdateVelocity();
            CheckCollisions();
            SetState();

        }
        else if (isGrounded)
        {
            _internalVelocity = Vector2.zero; _groundBounces = 0; 
            SetState();
        }
        if (isHit && isGrounded)
            MoveToInitialPosition();        
    }
    void GetState()
    {
        _internalPosition = transform.position;

        if (_internalVelocity == Vector2.zero)
            transform.position = startPosition;

    }
    void SetState()
    {
        _internalPosition += _internalVelocity;
        transform.position = _internalPosition;
    }
    void CheckCollisions()
    {
        int hits = Physics2D.BoxCastNonAlloc(transform.position,_boxCol.size,0,_internalVelocity, 
                                                _hitBuffer, _internalVelocity.magnitude, _validHits);
        Debug.Log("hits : " + hits);
        if (hits > 0)
        {
            Debug.Log("Hit");
            _internalVelocity.y = -_internalVelocity.y * 0.5f;
            _internalVelocity.x *= 0.5f;
            _groundBounces++;
            if (_groundBounces >= MAXGROUNDBOUNCES)
            {
                isGrounded = true;
                _internalVelocity = Vector2.zero;
            }
        }
    }
    void UpdateVelocity()
    {
        _internalVelocity.x = BeriMath.Accelerate(_internalVelocity.x,0,2.0f,Time.fixedDeltaTime);
        _internalVelocity.y = BeriMath.Accelerate(_internalVelocity.y,-5.0f,_gravity*_gravityScale,Time.fixedDeltaTime);
    }
    void MoveToInitialPosition()
    {
        transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position,(Vector3)startPosition) < 0.05f)
        {
            isHit = false;
            BattleManager.CurrentBattleManagerState = BattleManager.BattleManagerState.ENEMYTURN;
        }
    }
    public void SetVelocity(Vector2 inputVelocity)
    {
        isGrounded = false;
        isHit = true;
        _internalVelocity = inputVelocity/10.0f; 
    }
}

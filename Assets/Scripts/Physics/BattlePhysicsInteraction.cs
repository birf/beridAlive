using UnityEngine;
using BeriUtils.Core;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CharacterGameBattleEntity))]
public class BattlePhysicsInteraction : MonoBehaviour
{

    /*
        Script in charge for physics interactions during battle. interact with this script to launch enemies, move their position, etc.

        TODO : Migrate the blocking/parrying code into this script.
    */
    public enum CharacterPhysicsState
    {
        DEFAULT,
        HITSTUN,
        RECOVERY
    }
    
    public CharacterPhysicsState characterPhysicsState;

#region Primitives
    public bool isGrounded = true;
    public bool isHit = false;
    public bool jumping = false;
    public float localGroundYCoordinate; // <-- where in the world their current "ground floor" is 
    public int maxGroundBounces = 3;
    [Range(0.1f,50.0f)] public float moveSpeed = 10.0f;
    int _groundBounces = 5;
    float _gravity = 9.81f;
    float _lerpTime;
    const float MINMOVEDISTANCE = 0.01f;
#endregion

#region Classes and Structs
    public Vector3 startPosition = new Vector3();
    CharacterGameBattleEntity _characterBody;
    Vector3 _internalPosition = new Vector3();    
    Vector3 _internalVelocity = new Vector3();
    RaycastHit2D[] _hitBuffer = new RaycastHit2D[5];
    Collider2D[] _collisionBuffer = new Collider2D[3];
    [SerializeField] BoxCollider2D _boxCol;
    [SerializeField] LayerMask _validHits;
    [SerializeField][Range(0f,5.0f)] float _gravityScale = 2.0f;


#endregion
    void Awake()
    {
        _boxCol = GetComponent<BoxCollider2D>();
        _characterBody = GetComponent<CharacterGameBattleEntity>();
        if (_characterBody.characterScriptable != null)
            _boxCol.size = _characterBody.characterScriptable.battleHitBoxSize;
        isGrounded = true;
        startPosition = transform.position;
        localGroundYCoordinate = transform.position.y - 0.5f;

        characterPhysicsState = CharacterPhysicsState.DEFAULT;

    }

    void FixedUpdate()
    {
        GetState();

        // while the character isn't currently moving from hitstun.
        if (characterPhysicsState != CharacterPhysicsState.RECOVERY) 
        {
            // character is not currently grounded, and they are not in any blocking state.
            if (!isGrounded) 
            {
                UpdateVelocity();
                CheckCollisions();
                SetState();
            }
            else if (isGrounded) // character is grounded.
            {
                _internalVelocity = Vector2.zero; _groundBounces = 0;  jumping = false;
                characterPhysicsState = CharacterPhysicsState.DEFAULT;
                SetState();
            }
            if (isHit && isGrounded) // <-- tester just for now. 
            {
                characterPhysicsState = CharacterPhysicsState.RECOVERY;
            }
        }

        else
        {
            RecoverToInitialPosition(); // character is grounded and was just hit by something.                 
        }
    }
    void GetState()
    {
        _internalPosition = transform.position;
        _internalPosition.z = transform.position.y;

        // if (_internalVelocity == Vector2.zero)
        //     transform.position = startPosition;
    }
    void SetState()
    {
        _internalPosition += _internalVelocity;
        transform.position = _internalPosition;
    }
    void CheckCollisions() // <-- "collisions" in this case is just "are we above our ground coordinate?"
    {
        if ((transform.position.y + _internalVelocity.y) < localGroundYCoordinate)
        {
            _internalVelocity.y = -_internalVelocity.y * 0.5f;
            _internalVelocity.x *= 0.5f;
            _groundBounces++;
            if (jumping || (_groundBounces >= maxGroundBounces && !jumping))
                isGrounded = true;
        }
    }
    void UpdateVelocity()
    {
        _internalVelocity.x = BeriMath.Accelerate(_internalVelocity.x,0,2.0f,Time.fixedDeltaTime);
        _internalVelocity.y = BeriMath.Accelerate(_internalVelocity.y,-5.0f,_gravity*_gravityScale,Time.fixedDeltaTime);
    }
    void RecoverToInitialPosition()
    {
        MoveToPosition((Vector3)startPosition);
    }
    void MoveToPosition(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
        MoveGroundCoordinate(transform.position.y);
        // when character reaches the initial position, update their state.
        if (Vector3.Distance(transform.position,destination) < MINMOVEDISTANCE)
        {
            isHit = false;
            isGrounded = true;
            characterPhysicsState = CharacterPhysicsState.DEFAULT;
            localGroundYCoordinate = startPosition.y;
        }
    }

    public void HitTarget(Vector2 inputVelocity, int damage)
    {
        // called when we hit the target. check if we are blocking and set the appropriate damage.
        if (10 + _characterBody.characterData.curDEF > 99)
            return;
        _internalVelocity = inputVelocity/(10.0f + _characterBody.characterData.curDEF);
        characterPhysicsState = CharacterPhysicsState.HITSTUN;
        isGrounded = false;
        isHit = true;
        
        _characterBody.characterData.UpdateStat("Health", -damage);
    }
    public void MoveGroundCoordinate(float position)
    {
        localGroundYCoordinate = position;
    }
}

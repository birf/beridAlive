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
    public bool isLaunched = false;
    public bool isJumping = false;
    public bool shouldImmediatelyRecover = true;
    public float localGroundYCoordinate; // <-- where in the world their current "ground floor" is 
    public int maxGroundBounces = 3;
    [Range(0.1f,50.0f)] public float moveSpeed = 10.0f;
    int _groundBounces = 3;
    float _gravity = 9.81f;
    float _lerpTime;
    float _xLastFrame;
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
    [SerializeField] Animator _characterAnimator;


#endregion
    void Awake()
    {
        _boxCol = GetComponent<BoxCollider2D>();
        _characterBody = GetComponent<CharacterGameBattleEntity>();
        if (_characterBody.characterScriptable != null)
            _boxCol.size = _characterBody.characterScriptable.battleHitBoxSize;
        isGrounded = true;
        startPosition = transform.position;
        characterPhysicsState = CharacterPhysicsState.DEFAULT;
        _characterAnimator = GetComponent<Animator>();
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
                _internalVelocity = Vector2.zero; _groundBounces = 0;  isJumping = false;
                characterPhysicsState = CharacterPhysicsState.DEFAULT;
                SetState();
            }
            // if they've just been launched and are now grounded, recover to initial position.
            if (isLaunched && isGrounded && shouldImmediatelyRecover)  
            {
                _characterAnimator.Play("battle_idle");
                characterPhysicsState = CharacterPhysicsState.RECOVERY;
            }
        }

        else
        {
            RecoverToInitialPosition(); // character is grounded and was just hit by something.               
        }

        if (characterPhysicsState == CharacterPhysicsState.RECOVERY && _characterBody.characterData.curHP <= 0)
        {
            _characterBody.KillCharacterInBattle();  
        }
    }
    void GetState()
    {
        _internalPosition = transform.position;
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
            if (isJumping || (_groundBounces >= maxGroundBounces && !isJumping))
            {
                isGrounded = true;
                return;
            }
            _internalVelocity.y = -_internalVelocity.y * 0.5f;
            _internalVelocity.x *= 0.5f;
            _groundBounces++;
        }
    }
    void UpdateVelocity()
    {
        _internalVelocity.x = BeriMath.Accelerate(_internalVelocity.x,0,2.0f,Time.fixedDeltaTime);
        _internalVelocity.y = BeriMath.Accelerate(_internalVelocity.y,-5.0f,_gravity*_gravityScale,Time.fixedDeltaTime);
    }
    void RecoverToInitialPosition()
    {
        MoveToPosition((Vector3)startPosition, moveSpeed);
    }
    public void MoveToPosition(Vector3 destination, float speed = 10.0f)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
        _internalPosition = transform.position;
        MoveGroundCoordinate(transform.position.y);
        // when character reaches the initial position, update their state.
        if (Vector3.Distance(transform.position,destination) < MINMOVEDISTANCE)
        {
            isLaunched = false;
            isGrounded = true;
            characterPhysicsState = CharacterPhysicsState.DEFAULT;
            transform.position = destination;
        }
    }

    public void HitTarget(Vector2 inputVelocity, int damage)
    {
        // called when we hit the target. check if we are blocking and set the appropriate damage.
        if (damage <= 0)
            return;
        _internalVelocity = inputVelocity / 10.0f;
        characterPhysicsState = CharacterPhysicsState.HITSTUN;
        _groundBounces = 0;
        isGrounded = false;
        isLaunched = true;
        shouldImmediatelyRecover = true;

        _characterAnimator.Play("hurt");
        
        _characterBody.characterData.AddToStat(CharacterStat.HP, -damage, false);
    }
    public void LaunchTarget(Vector2 inputVelocity)
    {
        _internalVelocity = inputVelocity/(10.0f);
        characterPhysicsState = CharacterPhysicsState.HITSTUN;
        _groundBounces = 0;
        isGrounded = false;
        isLaunched = true;
    }
    public void Jump(Vector2 jumpVector)
    {
        LaunchTarget(jumpVector);
        isJumping = true;
    }
    public void Jump()
    {
        LaunchTarget(new Vector2(0f,2.5f));
        isJumping = true;
    }
    public void MoveGroundCoordinate(float position)
    {
        localGroundYCoordinate = position;
    }
    public void ResetToDefaultState()
    {
        characterPhysicsState = CharacterPhysicsState.DEFAULT;
        isJumping = false;
        isGrounded = true;
        isLaunched = false;
        shouldImmediatelyRecover = true;
    }
}

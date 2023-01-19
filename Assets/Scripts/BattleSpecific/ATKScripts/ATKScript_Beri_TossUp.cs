using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ATKScript_Beri_TossUp : ATKScript
{
    /*
        Attack script for the move 'Toss Up,' used by Beri.
        Beri launches her hand and drags opponents inward. Launches Upward.
    */
    [Range(1.0f,4.0f)]public float speedInverse = 1.5f;
    int subPhase = 0;

    ///<summary>
    ///Physical GameObject to grab the enemy.   
    ///</summary>
    public GameObject grabber;
    public BoxCollider2D safeArea;
    public BoxCollider2D grabberHitBox;
    [Range(1.0f, 25.0f)] public float grabberSpeed = 10.0f;
    
    PrimaryControls controls;
    [SerializeField] LayerMask validLayers;
    Vector3 _enemyGrabberInitialPosition;
    Vector3 _initialPosition;
    Vector3 _internalVelocity;
    Collider2D[] _hitBuffer = new Collider2D[3];
    CharacterGameEntity _grabbedEntity;

    void Awake()
    {
        _initialPosition = transform.position;
        controls = new PrimaryControls();
        controls.Enable();
        BeginMove();
    }
    void Update()
    {
        if (subPhase == 0)
        {
            GrabberMove();
            FirstPhase();
        }
        else if (subPhase == 1)
        {
            GrabberMove();
            SecondPhase();
        }
        else if (subPhase == 2)
            ThirdPhase();
    }
    void GrabberMove()
    {
        if (subPhase == 0)
            grabber.transform.position = Vector3.MoveTowards(grabber.transform.position,targetEnemy.transform.position, grabberSpeed * Time.deltaTime);
        else
            grabber.transform.position = Vector3.MoveTowards(grabber.transform.position,_initialPosition, grabberSpeed * Time.deltaTime);

    }
    void FirstPhase()
    {
        // First phase of the move. Send out the grabber and drag back as player holds enter. 
        // Failure if player hits the button and there is no target. 
        if (controls.Battle.Primary.triggered
            && subPhase == 0)
        {

            if (EnemyInGrabberBounds())
            {
                subPhase++;
                _grabbedEntity.transform.parent = grabber.transform;
                _enemyGrabberInitialPosition = grabber.transform.position;
                _grabbedEntity.characterBattlePhysics.isGrounded = true;
                _grabbedEntity.characterBattlePhysics.isHit = false;
            }
            else
                OnFailure();
        }
        if (Vector3.Distance(_initialPosition,grabber.transform.position) > Vector3.Distance(_initialPosition,targetEnemy.transform.position * 1.45f))
            OnFailure();
    }
    void SecondPhase()
    {
        // Second phase of the move. 
        // Drag back the enemy only if the player is still holding onto enter.
        if (controls.Battle.Primary.phase == InputActionPhase.Waiting)
        {
            if (EnemyInSafeArea())
            {
                subPhase++;  
                _grabbedEntity.transform.position = safeArea.transform.position + (Vector3)safeArea.offset;
                _grabbedEntity.transform.parent = null;
                Destroy(grabber); 
            }
            else
                OnFailure();
        }            
    }
    void ThirdPhase()
    {
        // Third phase. Throw up the opponent and deal damage. 

        if (controls.Battle.Direction.ReadValue<Vector2>().y == 1.0f)
        {
            targetEnemy.characterBattlePhysics.SetVelocity(parentMove.launchVelocity);
            targetEnemy.UpdateStat("Health",-parentMove.damage);
            _grabbedEntity.transform.parent = null;
            OnSuccess();
            Destroy(gameObject);
        }
    }
    bool EnemyInGrabberBounds()
    {
        // Is the grabber object within bounds of the enemy?
        int hits = Physics2D.OverlapBoxNonAlloc(grabberHitBox.transform.position + (Vector3)grabberHitBox.offset,grabberHitBox.size,0f,_hitBuffer,validLayers);
        if (hits == 0)
            return false;
        int i;
        for (i = 0; i < _hitBuffer.Length; i++)
        {
            if (_hitBuffer[i] != null)
            {
                if (_hitBuffer[i].gameObject.layer == 8 && _hitBuffer[i].gameObject == targetEnemy.gameObject)
                {
                    _grabbedEntity = targetEnemy;
                    return true;
                }
            }
        }
        return false;
    }
    bool EnemyInSafeArea()  
    {
        // Is the grabber object within bounds of the safe area?

        int hits = Physics2D.OverlapBoxNonAlloc(grabberHitBox.transform.position + (Vector3) grabberHitBox.offset,grabberHitBox.size,0f,_hitBuffer);
        if (hits == 0)
            return false;
        int i;
        for (i = 0; i < _hitBuffer.Length; i++)
        {
            if (_hitBuffer[i] != null)
            {
                if (_hitBuffer[i] == safeArea)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void BeginMove()
    {
        base.BeginMove();
    }
    public override void OnSuccess()
    {
        controls.Disable();
        battleManager.PlayerAttackSuccess();
        targetEnemy.transform.parent = null;
        base.OnSuccess();
        Destroy(gameObject);
    }
    public override void OnFailure()
    {
        
        if (targetEnemy.characterBattlePhysics.isHit)
            targetEnemy.characterBattlePhysics.SetVelocity(new Vector2(0,0));
        controls.Disable();
        battleManager.PlayerAttackFailure();
        targetEnemy.transform.parent = null;
        base.OnFailure();
        Destroy(gameObject);
    }
}

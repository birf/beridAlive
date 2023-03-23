using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BeriUtils.Core;
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
    Vector3 _initialPosition;
    Vector3 _internalVelocity;
    Collider2D[] _hitBuffer = new Collider2D[3];
    Timer timer = new Timer(3);

    void Awake()
    {
        _initialPosition = transform.position;
        timer.OnTimerEnd += OnFailure;
        controls = new PrimaryControls();
        controls.Enable();
        BeginMove();
    }
    protected override void Update()
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
        else if (subPhase >= 2)
            ThirdPhase();
    }
    void GrabberMove()
    {
        if (subPhase == 0)
            grabber.transform.position = Vector3.MoveTowards(grabber.transform.position, targetEnemy.transform.position, grabberSpeed * Time.deltaTime);
        else
            grabber.transform.position = Vector3.MoveTowards(grabber.transform.position,_initialPosition, grabberSpeed * Time.deltaTime);

    }
    void FirstPhase()
    {
        // First phase of the move. Send out the grabber and drag back as player holds enter. 
        // Failure if player hits the button and there is no target. 

        if (EnemyInGrabberBounds())
        {
            grabber.GetComponent<SpriteRenderer>().color = Color.green; // <-- tester.
        }

        if (controls.Battle.Primary.triggered && subPhase == 0)
        {
            if (EnemyInGrabberBounds())
            {
                grabber.GetComponent<SpriteRenderer>().color = Color.red; // <-- tester.
                grabber.transform.position = targetEnemy.transform.position;
                subPhase++;
                targetEnemy.transform.parent = grabber.transform;
                targetEnemy.characterBattlePhysics.isGrounded = true;
                targetEnemy.characterBattlePhysics.isLaunched = false;
            }
            else
                OnFailure();
        }
        if (Vector3.Distance(grabber.transform.position, targetEnemy.transform.position ) < 0.01f && subPhase == 0)
            OnFailure();
    }
    void SecondPhase()
    {
        // Second phase of the move. 
        // Drag back the enemy only if the player is still holding onto enter.

        targetEnemy.characterBattlePhysics.MoveGroundCoordinate(targetEnemy.transform.position.y - 0.5f);

        if (EnemyInSafeArea())
        {
            grabber.GetComponent<SpriteRenderer>().color = Color.green; // <-- tester.
        }
        if (controls.Battle.Primary.phase == InputActionPhase.Waiting)
        {
            if (EnemyInSafeArea())
            {
                subPhase++;  
                targetEnemy.transform.position = safeArea.transform.position + (Vector3)safeArea.offset;
                targetEnemy.transform.parent = null;
                Destroy(grabber); 
            }
            else
            {
                // player let go too early or late, drop enemy
                targetEnemy.characterBattlePhysics.isLaunched = true;
                OnFailure();
            }
        }

        // player reeled in enemy too close.
        if (Vector3.Distance(grabber.transform.position, _initialPosition) < 0.01f)
        {
            targetEnemy.characterBattlePhysics.HitTarget(new Vector2(-1f,-0.5f), 0);
            OnFailure();
        }       
    }
    void ThirdPhase()
    {
        // Third phase. Throw up the opponent and deal damage. 
        timer.Tick(Time.deltaTime);
        if (controls.Battle.Direction.ReadValue<Vector2>().y == 1.0f)
        {
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
                    return true;
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
        targetEnemy.transform.parent = null;
        Vector3 t = battleManager.currentActiveCharacter.transform.position;
        controls.Disable();
        base.OnSuccess();
        targetEnemy.characterBattlePhysics.MoveGroundCoordinate(t.y - 0.5f);
        targetEnemy.transform.parent = null;
        Destroy(gameObject);
    }
    public override void OnFailure()
    {
        
        if (targetEnemy.characterBattlePhysics.isLaunched)
            targetEnemy.characterBattlePhysics.HitTarget(new Vector2(-1f,0),0);
        
        else { targetEnemy.characterBattlePhysics.isLaunched = true; targetEnemy.characterBattlePhysics.isGrounded = true; }
        
        controls.Disable();
        battleManager.PlayerAttackFailure();
        targetEnemy.transform.parent = null;
        base.OnFailure();
        Destroy(gameObject);
    }
}

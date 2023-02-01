using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;
public class ATKScript_Tester_Shoot : ATKScript
{
    /*
        Tester enemy attack script. Just shoot some bullshit at the player lmao.
    */

    [SerializeField] BoxCollider2D _launcherHitbox;
    [SerializeField] [Range(0.1f, 20.0f)] float _launcherSpeed;
    Timer _launchTimer = new Timer(1);
    int subPhase = 0;

    void Awake()
    {
        _launchTimer.OnTimerEnd += Increment;
    }
    protected override void Update()
    {
        _launchTimer.Tick(Time.deltaTime);
        if (subPhase == 1)
            Move();
    }
    void Move()
    {
        _launcherHitbox.transform.position = Vector3.MoveTowards(_launcherHitbox.transform.position, targetEnemy.transform.position,
                                                                        Time.deltaTime * _launcherSpeed);
        if (CheckCollisions())
        {

            OnSuccess();
            Debug.Log("in first");
            Destroy(gameObject);
        }
    }
    bool CheckCollisions()
    {
        Collider2D[] buffer = new Collider2D[1];
        int hits = Physics2D.OverlapBoxNonAlloc(_launcherHitbox.transform.position,_launcherHitbox.size,0f,buffer,0x000000e0);
        if (hits > 0)
        {
            return true;
        }
        return false;
    }
    public override void OnSuccess()
    {
        base.OnSuccess();
        targetEnemy.characterBattlePhysics.HitTarget(parentMove.mainLaunchVelocity, parentMove.damage);
        battleManager.currentActiveCharacter.GetComponent<BasicEnemyAI>().canExecute = true;
    }
    public override void OnFailure()
    {
        base.OnFailure();
    }
    void Increment()
    { subPhase++; }
}

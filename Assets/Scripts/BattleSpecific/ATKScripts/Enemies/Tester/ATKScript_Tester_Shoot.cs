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
    [SerializeField] LayerMask _validLayers;
    Timer _launchTimer = new Timer(1);
    int subPhase = 0;


    private void OnEnable()
    {
        _launchTimer.OnTimerEnd += PlayShootSFX;
    }
    private void OnDisable()
    {
        _launchTimer.OnTimerEnd -= PlayShootSFX;
    }


    void PlayShootSFX()
    {
        FindObjectOfType<BattleManager>().GetComponent<AudioSource>().Stop();
        FindObjectOfType<BattleManager>().GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.FIREPROJECTILE);
    }


    void Awake()
    {
        _launchTimer.OnTimerEnd += Increment;
        BeginMove();
    }
    protected override void Update()
    {
        _launchTimer.Tick(Time.deltaTime);

        if (!FindObjectOfType<BattleManager>().GetComponent<AudioSource>().isPlaying)
        {
            FindObjectOfType<BattleManager>().GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.CHARGING_PROJECTILE);
        }

        Debug.Log("Charging");
        if (subPhase == 1)
            Move();
    }
    void Move()
    {
        _launcherHitbox.transform.position = Vector3.MoveTowards(_launcherHitbox.transform.position, targetEnemy.transform.position,
                                                                        Time.deltaTime * _launcherSpeed);

        if (targetEnemy.GetComponent<BlockScript>().CheckCollisions(out int damageReduction))
        {
            OnSuccess(parentMove.damage - damageReduction);
            Destroy(gameObject);
        }
    }
    public override void OnSuccess()
    {
        base.OnSuccess();
    }
    public override void OnFailure()
    {
        base.OnFailure();
    }
    void Increment()
    { subPhase++; }
}

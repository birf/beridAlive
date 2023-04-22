using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKScript_GhostSlash : ATKScript
{
    // bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad lmao 
    [SerializeField] GameObject slashAnimation;
    [SerializeField] BoxCollider2D slashHitBox;

    float startTime = 0.7088f;
    bool started = false;
    bool alreadyHit = false;

    void Awake()
    {
        BeginMove();
    }
    protected override void Update()
    {
        startTime -= Time.deltaTime;
        if (startTime <= 0 && started == false)
        {
            started = true;
            foreach(Animator anim in slashAnimation.GetComponentsInChildren<Animator>())
            {
                anim.Play("ghost_slashSlash");
            }
            foreach(SpriteRenderer sprite in slashAnimation.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,1);
            }
            startTime = 0.1666f;
        }
        if (started && startTime >= 0.08f)
        {
            slashHitBox.transform.position = targetEnemy.transform.position;
            CheckCollisions();
            if (!alreadyHit)
                slashHitBox.gameObject.SetActive(true);
            
        }
        if (alreadyHit && startTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    void CheckCollisions()
    {
        if (targetEnemy.GetComponent<BlockScript>().CheckCollisions(out int damageReduction))
        {
            base.OnSuccess(parentMove.damage - damageReduction);
            slashHitBox.gameObject.SetActive(false);
            alreadyHit = true;
        }
    }
    public override void BeginMove()
    {
        base.BeginMove();
        slashAnimation.transform.position = targetEnemy.transform.position;
        foreach(SpriteRenderer sprite in slashAnimation.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,0);
        }
    }
}

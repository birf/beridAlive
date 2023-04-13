// its all bad, i can't code. i can't do it. can't do anything right.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleOpeningMovie : MonoBehaviour
{
    [SerializeField] Animator vsAnimation;
    [SerializeField] Animator shakerAnimation;
    [SerializeField] Animator whiteBackgroundAnimation;
    [SerializeField] Animator whiteForegroundAnimation;
    [SerializeField] Animator decagramLeftAnimation;
    [SerializeField] Animator decagramRightAnimation;

    [SerializeField] GameObject theDotOverlordToOneDayRuleUsAll;
    [SerializeField] GameObject mainParent;
    [SerializeField] BattleCharacterPortrait beriPortrait;
    [SerializeField] BattleCharacterPortrait[] enemyPortraits = new BattleCharacterPortrait[5];
    
    [SerializeField][Range(0.5f,10.0f)] float dotSpeed = 0.5f;
    
    public List<CharacterScriptable> enemiesToDisplay = new List<CharacterScriptable>();
    float generalAnimationTimer = 0f;
    float spawnTimer = 0f;
    int listIndex = 0;
    
    [SerializeField][Range(0f,10f)] float versusStartTime = 0f;
    [SerializeField][Range(0f,10f)] float decagramStartTime = 0f;
    [SerializeField][Range(0f,10f)] float beriPortraitEntranceTime = 0f;
    [SerializeField][Range(0f,10f)] float enemyPortraitEntranceTime = 0f;
    [SerializeField][Range(0f,10f)] float foregroundFadeinTime = 0f;
    [SerializeField][Range(0f,10f)] float putAwayTime = 0f;
    [SerializeField][Range(0f,10f)] float killTime = 0f;

    void OnEnable()
    {
        whiteBackgroundAnimation.Play("white_open");
        whiteBackgroundAnimation.GetComponent<SpriteRenderer>().color = Color.white;
        mainParent.SetActive(true);

    }
    void OnDisable()
    {
        enemiesToDisplay.Clear();
        for (int i = 0; i < enemyPortraits.Length; i++)
        {
            enemyPortraits[i].transform.position = enemyPortraits[i].startPosition;
            enemyPortraits[i].mainSprite.color = Color.black;
            enemyPortraits[i].gameObject.SetActive(false);
        }

        beriPortrait.transform.position = beriPortrait.startPosition;
        beriPortrait.mainSprite.color = Color.black;
        beriPortrait.gameObject.SetActive(false);

        generalAnimationTimer = 0f;
        listIndex = 0;

        vsAnimation.gameObject.SetActive(false);

        decagramLeftAnimation.gameObject.SetActive(false);
        decagramRightAnimation.gameObject.SetActive(false);

        theDotOverlordToOneDayRuleUsAll.transform.position = new Vector3(14.54f,-13.4f,0);
        theDotOverlordToOneDayRuleUsAll.SetActive(false);

        
    }
    void Update()
    {
        generalAnimationTimer += Time.deltaTime;
        if (generalAnimationTimer >= versusStartTime)
            VersusAnimation();
        if (generalAnimationTimer >= decagramStartTime)
            DecagramAnimation();
        if (generalAnimationTimer >= beriPortraitEntranceTime)
            BeriPortraitAnimation();
        if (generalAnimationTimer >= enemyPortraitEntranceTime)
            EnemyPortraitAnimations();
        if (generalAnimationTimer >= foregroundFadeinTime && generalAnimationTimer < putAwayTime)
            ForegroundFadeinAnimation();
        if (generalAnimationTimer >= (foregroundFadeinTime + 1.33f))
        {
            theDotOverlordToOneDayRuleUsAll.SetActive(true);
        
            Vector3 newPos = theDotOverlordToOneDayRuleUsAll.transform.position;
            theDotOverlordToOneDayRuleUsAll.transform.position = 
                new Vector3(newPos.x - dotSpeed * Time.deltaTime, newPos.y + dotSpeed * Time.deltaTime, newPos.z);
            
            shakerAnimation.Play("shakeItBaby");
            beriPortrait.mainSprite.color = Color.white;
            for (int i = 0; i < enemyPortraits.Length; i++)
            {
                enemyPortraits[i].mainSprite.color = Color.white;
            }
        }
        if (generalAnimationTimer >= putAwayTime)
            PutAway();
        if (generalAnimationTimer >= putAwayTime + 0.5f)
        {   
            mainParent.SetActive(false);
        }
        if (generalAnimationTimer >= killTime)
            gameObject.SetActive(false);
    }
    void VersusAnimation()
    { 
        vsAnimation.gameObject.SetActive(true);
        vsAnimation.Play("vs_splash"); 
        vsAnimation.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1); 
    }
    void DecagramAnimation()
    {
        decagramLeftAnimation.gameObject.SetActive(true);
        decagramRightAnimation.gameObject.SetActive(true);

        decagramLeftAnimation.Play("decagram_left");
        decagramRightAnimation.Play("decagram_right");
    }
    void BeriPortraitAnimation()
    {
        beriPortrait.gameObject.SetActive(true);
    }
    void EnemyPortraitAnimations()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= 0.2f && listIndex < enemiesToDisplay.Count)
        {
            enemyPortraits[listIndex].characterScriptable = enemiesToDisplay[listIndex];
            enemyPortraits[listIndex].gameObject.SetActive(true);
            listIndex++;
            spawnTimer = 0;
        }
    }
    void ForegroundFadeinAnimation()
    { whiteForegroundAnimation.Play("foreground_white_fadein"); }
    void PutAway()
    { whiteForegroundAnimation.Play("fadeout"); }

}


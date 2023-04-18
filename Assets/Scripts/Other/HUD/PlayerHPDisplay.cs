using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BeriUtils.Core;
public class PlayerHPDisplay : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    [SerializeField] PlayerTurnUI playerTurnUI;
    [SerializeField] Animator animator;
    [SerializeField] TextMeshPro mainText;
    [SerializeField] TextMeshPro smallText;

    public int curHealth;
    public int maxHealth;
    int healthLastFrame;

    void Awake()
    {
        if (!battleManager)
            battleManager = (BattleManager)CentralManager.GetStateManager();
        if (!playerTurnUI)
            playerTurnUI = FindObjectOfType<PlayerTurnUI>();
        if (!animator || !mainText || !smallText)
        {
            Debug.Log("ERROR : Player HP Display lacks all components.");
            Destroy(gameObject);
        }
    }

    void CheckForDamage()
    {
        if (BattleManager.CurrentBattleManagerState != BattleManager.BattleManagerState.DEFAULT)
        {
            curHealth = battleManager.playerCharacters[0].characterData.curHP;
            maxHealth = battleManager.playerCharacters[0].characterData.baseHP;
        }
        SetText();
    }
    void UpdateAnimationState()
    {
        float ratio = (float) curHealth / (float) maxHealth;
        if (ratio <= 0.4f)
        {
            if (curHealth <= 1)
                animator.Play("peril");
            else
                animator.Play("low_health",1);
        }
        else
            animator.Play("none",1);
        
        if (healthLastFrame > curHealth)
        {
            animator.Play("damage",0);
            healthLastFrame = curHealth;
        }
        
        if (playerTurnUI.currentState == "Attack")
            animator.Play("put_away",0);
        if ((playerTurnUI.currentState == "ActionSelect" || playerTurnUI.currentState == "AttackTargetSelection")
        &&  !animator.GetCurrentAnimatorStateInfo(0).IsName("none"))
            animator.Play("ready");
    }
    void Update()
    {
        CheckForDamage();
        UpdateAnimationState();
    }
    void LateUpdate() 
    {
        healthLastFrame = curHealth;
    }

    void SetText()
    {
        mainText.text = curHealth.ToString();
        smallText.text = "/ " + maxHealth;
    }

}

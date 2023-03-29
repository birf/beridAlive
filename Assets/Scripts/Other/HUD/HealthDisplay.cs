using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
class HealthUI
{
    [SerializeField] private TMP_Text health;
    public TMP_Text Health => health;
}

public class HealthDisplay : MonoBehaviour
{

    [SerializeField] BattleManager battleManager;
    //[SerializeField] List<HealthUI> healthUI;

    [SerializeField] List<TMP_Text> playerHealthText;
    [SerializeField] List<TMP_Text> enemyHealthText;

    [SerializeField] List<HudEntity> hudEntities;


    public void InitializeHealthUI()
    {
        battleManager.playerCharacters[0].gameObject.AddComponent<HudEntity>().SetHUDEntity(playerHealthText[0]);
        hudEntities.Add(battleManager.playerCharacters[0].GetComponent<HudEntity>());
        for (int i = 0; i < battleManager.enemyCharacters.Count; i++)
        {
            battleManager.enemyCharacters[i].gameObject.AddComponent<HudEntity>().SetHUDEntity(enemyHealthText[i]);
            hudEntities.Add(battleManager.enemyCharacters[i].GetComponent<HudEntity>());
        }

        for (int i = 0; i < hudEntities.Count; i++)
        {
            hudEntities[i].SetupText();
        }
    }

    public void UpdateHealthUI()
    {

        for (int i = 0; i < hudEntities.Count; i++)
        {
            hudEntities[i].UpdateHealthText();
        }

        //playerHealthText[0].gameObject.SetActive(true);
        //battleManager.playerCharacters[0].gameObject.AddComponent<HudEntity>().SetHUDEntity(playerHealthText[0]);
        ////UpdateText(playerHealthText[0], battleManager.playerCharacters[0], 0);
        //for (int i = 0; i < battleManager.enemyCharacters.Count; i++)
        //{
        //    enemyHealthText[i].gameObject.SetActive(true);
        //    battleManager.enemyCharacters[i].gameObject.AddComponent<HudEntity>().SetHUDEntity(enemyHealthText[i]);
        //    //CreateText(enemyHealthText[i], battleManager.enemyCharacters[i], 1);
        //}
    }

    public void RemoveHudEntity(HudEntity entity)
    {
        entity.tMP_Text.gameObject.SetActive(false);
        hudEntities.Remove(entity);
    }

    public void RemoveAllHudEntities()
    {

        for (int i = 0; i < hudEntities.Count; i++)
        {
            HudEntity entity = hudEntities[i].GetComponent<HudEntity>();
            hudEntities.Remove(entity);
            Destroy(entity);

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateHealthUI();
        }
    }
}

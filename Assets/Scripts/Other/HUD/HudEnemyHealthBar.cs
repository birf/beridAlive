using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HudEnemyHealthBar : MonoBehaviour
{
    public int curHealth;
    public int maxHealth;

    [SerializeField] SpriteMask mask;
    [SerializeField] TextMeshPro numberText;

    void Awake()
    {
        if (maxHealth == 0 || curHealth == 0)
        {
            Debug.Log("health value not set!");
            Destroy(gameObject);
        }
    }
    void Update()
    {
        curHealth = GetComponentInParent<CharacterGameBattleEntity>().characterData.curHP;
        curHealth = Mathf.Clamp(curHealth,0,maxHealth);
        float ratio = (float)curHealth/(float)maxHealth;

        Vector3 newScale = new Vector3(-(1-ratio),1,1);

        mask.transform.localScale = newScale;
        
        numberText.text = curHealth.ToString();
    }
}

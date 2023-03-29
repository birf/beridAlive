using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudEntity : MonoBehaviour
{

    [SerializeField] HealthDisplay healthDisplay;
    [SerializeField] public TMP_Text tMP_Text;



    //public static event Action On

    public void SetHUDEntity(TMP_Text text)
    {
        healthDisplay = FindObjectOfType<HealthDisplay>();
        tMP_Text = text;
    }

    public void SetText(string text)
    {
        tMP_Text.text = text;
    }
    public void UpdateText()
    {

    }


    public void SetupText()
    {
        tMP_Text.gameObject.SetActive(true);
        tMP_Text.transform.parent = transform;
        tMP_Text.transform.localPosition = Vector3.zero;
        tMP_Text.text = GetComponent<CharacterGameBattleEntity>().characterData.curHP.ToString();
        tMP_Text.transform.localPosition = new Vector2(0, tMP_Text.transform.localPosition.y - 3);

        if (GetComponent<CharacterGameBattleEntity>().characterData.CharType == CharacterBase.CharacterType.PLAYER)
        {

            tMP_Text.transform.parent = healthDisplay.transform.GetChild(0).transform.parent;
        }
        else
        {
            tMP_Text.transform.parent = healthDisplay.transform.GetChild(1).transform.parent;
        }
    }


    public void UpdateHealthText()
    {
        tMP_Text.text = GetComponent<CharacterGameBattleEntity>().characterData.curHP.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

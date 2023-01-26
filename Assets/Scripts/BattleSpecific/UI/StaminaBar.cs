using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StaminaBar : MonoBehaviour
{
    public GameObject fillBar;
    public UISelectable GOButton;
    public List<SpriteRenderer> fillBarClones = new List<SpriteRenderer>(); 
    public Vector3 targetPosition;
    public int staminaCount = 1;
    public float currentLerpTime = 0f;

    [SerializeField][Range(0.5f,50.0f)] float lerpSpeed = 1.0f;
    [SerializeField] GameObject goButtonObject;
    [SerializeField] TextMeshPro goButtonText;
    [SerializeField] SpriteRenderer goButtonBackground;

    void OnEnable()
    {
        InstantiateBar();
        GOButton.gameObject.name = "GO!";
    }
    void Update()
    {
        Move();
        GOButton.cursorTarget = GOButton.transform.position + new Vector3(3f,0,0);
        UpdateGoButton();
    }
    void UpdateGoButton()
    {        
        if (!GOButton.isSelectable)
        {
            goButtonBackground.color = new Color(0,1,0,0.5f);
            goButtonText.color = new Color(1,1,1,0.5f);
        }
        else
        {
            goButtonBackground.color = new Color(0,1,0,1);
            goButtonText.color = new Color(1,1,1,1);
        }
    }
    public void UpdateStaminaBar(int input)
    {
        for (int i = 0; i < input; i++)
            fillBarClones[i].color = Color.white;
        
        for (int j = input; j < fillBarClones.Count; j++)
            fillBarClones[j].color = new Color(1,1,1,0.5f);

    }
    void OnDisable()
    {
        transform.position += new Vector3(-20,0,0);
        for (int i = 0; i < fillBarClones.Count; i++)
            Destroy(fillBarClones[i].gameObject);
        fillBarClones.Clear();
    }
    void InstantiateBar()
    {
        int height = 12;
        Vector3 startingPosition = transform.position + new Vector3(0,-6,0);
        
        height /= staminaCount == 0 ? 1 : staminaCount;

        for (int i = 0; i < staminaCount; i++)
        {
            GameObject obj = Instantiate(fillBar,startingPosition + new Vector3(0,(float)(6 / (float)staminaCount), 0),Quaternion.identity,transform);
            SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
            rend.size = new Vector2(1.333f, 12 / (float)staminaCount);
            rend.sortingOrder = 100;
            rend.color = new Color(1,1,1,0.5f); // initiallly render each at half opacity.
            startingPosition += new Vector3(0, 12 / (float)staminaCount, 0);
            fillBarClones.Add(rend);
        }
        UpdateStaminaBar(staminaCount);
    }
    void Move()
    {
        transform.position = Vector3.Lerp(transform.position,targetPosition,Time.deltaTime * lerpSpeed);
    } 
}

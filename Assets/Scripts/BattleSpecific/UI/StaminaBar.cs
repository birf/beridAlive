using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    public GameObject fillBar;
    public UISelectable GOButton;
    public List<SpriteRenderer> fillBarClones = new List<SpriteRenderer>(); 
    public Vector3 targetPosition;
    public int staminaCount = 1;
    [SerializeField][Range(0.5f,6.0f)] float lerpSpeed = 1.0f;
    public float currentLerpTime = 0f;


    void OnEnable()
    {
        InstantiateBar();
        GOButton.gameObject.name = "GO!";
    }
    void Update()
    {
        Move();
        GOButton.cursorTarget = GOButton.transform.position + new Vector3(3f,0,0);
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
        
        height /= staminaCount == 0 ? staminaCount : 1;

        for (int i = 0; i < staminaCount; i++)
        {
            GameObject obj = Instantiate(fillBar,startingPosition + new Vector3(0,(float)(6 / (float)staminaCount), 0),Quaternion.identity,transform);
            SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
            rend.size = new Vector2(1.333f, 12 / (float)staminaCount);
            rend.sortingOrder = 100;
            startingPosition += new Vector3(0, 12 / (float)staminaCount, 0);
            fillBarClones.Add(rend);
        }
    }
    void Move()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpSpeed)
            currentLerpTime = 0;
        float percentage = currentLerpTime/lerpSpeed;
        transform.position = Vector3.Lerp(transform.position,targetPosition,percentage);
    } 
}

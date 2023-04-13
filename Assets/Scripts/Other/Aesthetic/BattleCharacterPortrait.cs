using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BattleCharacterPortrait : MonoBehaviour
{
    
    public CharacterScriptable characterScriptable;
    public SpriteRenderer mainSprite;
    public SpriteRenderer shadowSprite;
    public Vector3 startPosition;
    public Vector3 endPositionDifference;
    [Range(0.1f,50f)] public float lerpSpeed = 5.0f;
    void OnEnable()
    {
        if (characterScriptable)
        {
            GetComponent<SpriteRenderer>().sprite = characterScriptable.characterSprites[0];
            shadowSprite.sprite = characterScriptable.characterSprites[0];
            mainSprite.sprite = characterScriptable.characterSprites[0];
            shadowSprite.color = new Color(0,0,0,0.5f);
        }
        else
        {
            Debug.Log("ERROR : No Character Scriptable loaded for this character portrait!");
        }
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,startPosition + endPositionDifference,Time.deltaTime * lerpSpeed);
    }
}

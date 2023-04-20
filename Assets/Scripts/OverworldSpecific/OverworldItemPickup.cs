using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;


[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class OverworldItemPickup : MonoBehaviour
{
    [SerializeField] OverworldManager overworldManager;
    [SerializeField] ItemData itemPickup;
    [SerializeField] SpriteRenderer spriteObject;
    [SerializeField] Camera activeCamera;

    Timer killTimer;

    [SerializeField] bool itemHasBeenPickedUp;
    void Awake()
    {
        itemPickup.GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
        spriteObject.sprite = sprites[1];

        overworldManager = FindObjectOfType<OverworldManager>();
        killTimer = new Timer(2);
        killTimer.OnTimerEnd += DestroyObject;
    }
    void Update()
    {
        if (itemHasBeenPickedUp)
        {
            killTimer.Tick(Time.deltaTime);
            spriteObject.transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(spriteObject.transform.position.y,transform.position.y, 50f * Time.deltaTime),
                transform.position.z
            );
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CharacterGameOverworldEntity>(out CharacterGameOverworldEntity entity))
        {
            Debug.Log("woah");
            overworldManager.TogglePlayerController(false);
            entity.characterData.items.Add(itemPickup);
            GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
            spriteObject.gameObject.SetActive(true);
            itemHasBeenPickedUp = true;
            GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.SELECT);
        }
    }
    void DestroyObject()
    {
        Destroy(gameObject);
        overworldManager.TogglePlayerController(true);
    }
}

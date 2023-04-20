using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelExit : MonoBehaviour
{

    public OverworldManager overworldManager;

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (overworldManager.activeLevel.GetComponent<OverworldLevel>().canExit())
            GetComponent<SpriteRenderer>().color = Color.cyan;
        else
            GetComponent<SpriteRenderer>().color = Color.black;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag != "Player") return;
        if(overworldManager.activeLevel.GetComponent<OverworldLevel>().canExit()){   
            overworldManager.StartNextLevel();    
        }
        
    }
}

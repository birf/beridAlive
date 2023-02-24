using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldLevel : MonoBehaviour
{
    [SerializeField] Vector3 playerOrigin;

    public OverworldManager overworldManager;
    private List<GameObject> enemies;

    public List<GameObject> getEnemies(){ return enemies;}
    private bool levelInitialized = false;

    //initialize the list of enemies
    private void initializeEnemies(){
        if(enemies == null) enemies = new List<GameObject>();
        Transform enemiesTransform = transform.Find("enemies");
        foreach(Transform child in enemiesTransform){           
            enemies.Add(child.gameObject);
        }

    }

    public void initializeLevel(){
        initializeEnemies();
        GameObject.Find("player").transform.position = playerOrigin;
        levelInitialized = true;
    }

    //returns false if there are enemies still in the level
    public bool canExit(){
        //Debug.Log("canExit");
        if(!levelInitialized) return false;
        foreach(GameObject enemy in enemies){
            if(enemy != null) return false;
        }
        return true;        
    }


    void Start()
    {

    }

    void OnEnable(){

    }

    void Update()
    {

        
    }
}

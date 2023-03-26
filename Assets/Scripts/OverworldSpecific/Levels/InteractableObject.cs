using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableObject: MonoBehaviour
{

    //public GameObject t; //DELETE THIS
    public GameObject player; //a reference to the player

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //activeLevel.GetComponent<OverworldLevel>().
    //determine which side the character interacted with the object from
    public Vector3 getSide(){
	float xdif = player.transform.position.x - this.transform.position.x;
	float ydif = player.transform.position.y - this.transform.position.y;
	if(xdif < 0 && ydif < 0){ return Vector3.down;} //bottom left
	if(xdif < 0  && ydif >= 0){ return Vector3.left;} //top left
	if(xdif >= 0 && ydif >= 0){ return Vector3.up;} //top right
	if(xdif >= 0 && ydif < 0){return Vector3.right;} //bottom right
	return Vector3.up;
	
	
    }

    //implement in subclasses
    public virtual void onInteract(){
	    return;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
	//Debug.Log("Trigger");
	onInteract(); //for now, just call interact on any collision
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script is attached to the iceblock, handles all puzzle logic
public class Iceblock : InteractableObject
{
    
    public GameObject hole; //hole that gets filled when colliding with the block
    public GameObject block; //the iceblock
    [Range(0.5f,20.0f)]public float speed; //sliding speed of the block 
    public bool isMoving; //is the block currently sliding
    public Vector3 moveVector; //direction block is sliding in
    



    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            int colliderLayer = 9; 
            int colliderlayerMask = 1 << colliderLayer; 

            int holeLayer = 10;
            int holelayerMask = 1 << holeLayer; 
            
            //ice_collisions layer
            if(Physics2D.Raycast(transform.position, moveVector, 0.25f, colliderlayerMask)){
                isMoving = false;
            }
            //hole layer
            if(Physics2D.Raycast(transform.position, moveVector, 0.01f, holelayerMask)){
                hole.SetActive(false);
                isMoving = false;
                block.SetActive(false);
            }

            if(isMoving) this.transform.position += speed * moveVector; 

        }
    }

     public override void onInteract(){
        
	    if(player == null) return;
        Vector3 side = getSide(); //get the side of the block player interacted with

        //find a movement direction based on the side
        if(side == Vector3.up){
            moveVector= new Vector3(-0.5f, -0.25f, 0);
        }
        if(side == Vector3.right){
            moveVector= new Vector3(-0.5f, 0.25f, 0);
        }
        if(side == Vector3.down){
            moveVector= new Vector3(0.5f, 0.25f, 0);
        }
        if(side == Vector3.left){
            moveVector = new Vector3(0.5f, -0.25f, 0);
        }

        moveVector *= Time.deltaTime;
        isMoving = true;

    }

}

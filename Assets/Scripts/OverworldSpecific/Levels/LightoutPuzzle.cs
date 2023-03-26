using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightoutPuzzle : MonoBehaviour
{   public GameObject baseObject;
    public LightoutBlock[,] blocks;
    public float startx; //x and y coordinates of the first block in the puzzle
    public float starty;   
    public int dimension; //the dimension of the puzzle (e.g. 3x3)
    public int gapSize; //how many tiles between each block
    public GameObject gate; //gate that gets deleted when the puzzle is solved
    public GameObject gateCollider; //collider for the gate

    //logic for when player interacts with blocks[x][y]
    public void handleInteraction(int x, int y){
        //swap the state of the block and the 4 cardinally adjacent blocks
        for(int i = 0; i < 3; i++){
            int candidateX = x + i + -1;
            if(candidateX >= 0  && candidateX < dimension){ //array bounds checking
                blocks[candidateX, y].swapActive();
            }
        }
        for(int i = 0; i < 3; i++){
            if(i == 1) continue; //this block was swapped in 1st loop
            int candidateY = y + i + -1;
            if(candidateY >= 0  && candidateY < dimension){ 
                blocks[x, candidateY].swapActive();
        }
        }
        //check if puzzle is now solved
        if(isSolved()){
            onSolved();
        }
    }

    //return true if puzzle is solved
    public bool isSolved(){
        //if any block in the puzzle is not active, return false.
        for(int i = 0; i < dimension; i++){
            for(int j = 0; j <dimension; j++){
                if(!blocks[i, j].isActive){
                    return false;
                }
            }
        }
        return true;
    }

    //when puzzle is solved
    public void onSolved(){
        //disable the gate and its collider
        gate.SetActive(false);
        gateCollider.SetActive(false);
    }

    //create the puzzle
    void Start(){
        blocks = new LightoutBlock[dimension, dimension];
        gapSize++; //increment to avoid off by 1 error in logic below
        for(int i = 0; i < dimension; i++){ //create the dimension X dimension grid
            for (int j=0; j<dimension; j++){
                GameObject newBlock = Instantiate(baseObject, 
                    //for now, the offset coords are hardcoded
                    new Vector3(startx + i*(0.5f*gapSize) + j*(-0.5f*gapSize), starty + i*(-0.25f*gapSize) + j*(-0.25f*gapSize), 0), 
                    Quaternion.identity, this.transform);
                newBlock.SetActive(true);
                LightoutBlock lb = newBlock.GetComponent<LightoutBlock>();
                lb.xcoord = i; lb.ycoord = j;
                blocks[i,j] = lb;
                
            }
        }

    }
}

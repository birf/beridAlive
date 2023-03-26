using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightoutBlock : InteractableObject
{
    //coordinates in the  puzzle's 2d array
    public int xcoord; 
    public int ycoord;
    public LightoutPuzzle puzzle; //reference to the puzzle
    public bool isActive;



    public LightoutBlock(int x, int y, LightoutPuzzle newPuzzle){
        xcoord = x;
        ycoord = y;
        puzzle = newPuzzle;
    
    }

    //flip the value of isActive and update the sprite
    public void swapActive(){
        isActive = !isActive;
        
        //change sprite, for now just recolor
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if(isActive){
            renderer.color = Color.green;
        }
        else{
            renderer.color = Color.red;
        }
    }

    public override void onInteract(){
        puzzle.handleInteraction(xcoord, ycoord);
    }
}

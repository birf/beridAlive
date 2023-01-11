using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    /*
        Game managers are physical objects in the world that are responsible for activating and deactivating states.
        Each should contain a list of states to dictate the current state of the game.
        
        Keep in mind the 'ChildObjects' and other entities that the manager has access to, and make sure to update 
        them accordingly as progression continues (i.e., update the items in each list to reflect the game world).
    */

    ///<summary>
    // Publicly accessible list of sister objects for states to manipulate / activate.
    // Sister objects can include anything from UI elements to other portions of the
    // game, including another state manager.
    ///</summary>
    public List<GameObject> ChildObjects = new List<GameObject>();
    
    ///<summary>
    /// Publicly accessible list of character objects associated with this state manager.
    ///</summary>
    public List<CharacterBase> Characters = new List<CharacterBase>();
    
    ///<summary>
    ///Character Game Entities associated with this state manager.
    ///</summary>
    public List<CharacterGameEntity> CharacterGameObjects = new List<CharacterGameEntity>();  
}

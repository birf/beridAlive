public static class CentralManager 
{
    /*
        Static class used for global communication with the active state manager. 
    */
    static GameManager centralManager;

    ///<summary>
    ///The context in which the current game manager is in. This will be used to correctly cast the central manager to the right 
    ///type in other objects concerned with it so that it may take advantage of the appropriate function calls.
    ///</summary>

    public enum Context
    {
        OVERWORLD,
        BATTLE
    }
    public static Context CurrentContext;
    public static GameManager GetStateManager()
    {
        return centralManager;
    }
    public static void SetStateManager(GameManager input)
    {
        centralManager = input;
    }
}

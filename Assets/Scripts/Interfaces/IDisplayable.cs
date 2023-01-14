using UnityEngine;
///<summary>
///Data that can be displayed under any given context.
///</summary>
public interface IDisplayable 
{
    ///<summary>
    ///Return display data. Order of elements matters : 
    ///
    ///     Sprite[] sprites | lower indicies represent smaller sprites, grows larger as index increases. ||
    ///
    ///           int[] ints | order indicates importance. 0 -> Primary, 1-> Secondary, 2 -> Tertiary etc. -1 at first index should indicate to not use this data.||
    ///
    ///     string[] strings | order indicates imporance. 0 -> Display name, 1 -> Display description, etc. ||
    ///
    ///</summary>
    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings);
}

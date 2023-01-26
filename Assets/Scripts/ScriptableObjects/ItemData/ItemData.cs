using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable/Item Data")]
public class ItemData : ScriptableObject , IDisplayable
{
    /*
        Item data for easily creating new item objects. Everything here should be readonly.
    */
    public Sprite itemSpriteSmall;
    public int potency;
    public string itemName;
    public string itemDesc;
    public ItemType itemType;
    public enum ItemType // <-- no idea what to use this for yet lmao
    {
        HELPER
    }

    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = new Sprite[] {itemSpriteSmall};
        ints = new int[] {-1,-1,potency};
        strings = new string[] {itemName,itemDesc};
    }
    public void UseItem(CharacterBase character, string effect)
    {
        character.UpdateStat(effect, potency);
    }
}

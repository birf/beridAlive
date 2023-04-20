using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable/Item Data")]
public class ItemData : ScriptableObject , IDisplayable
{
    /*
        Item data for easily creating new item objects. Everything here should be readonly.
    */
    public Sprite itemSpriteSmall;
    public Sprite itemSpriteLarge;
    public int potency;
    public string itemName;
    public string itemDesc;
    public CharacterStat statAffected;
    public int duration;
    public bool inflictsStatusEffect;

    public void GetDisplayData(out Sprite[] sprites, out int[] ints, out string[] strings)
    {
        sprites = new Sprite[] {itemSpriteSmall,itemSpriteLarge};
        ints = new int[] {-1,-1,potency};
        strings = new string[] {itemName,itemDesc};
    }

    // TODO : Make it so that the player can't use items if it isn't necessary to do so. 
    public void UseItem(CharacterBase character, CharacterStat effect)
    {
        if (!inflictsStatusEffect)
            character.AddToStat(effect, potency, false);
        else
            character.statusEffects.Add(new CharacterStatusEffect(duration,potency,character.GetStatValueByStatType(effect,true),effect,character));
    }
}

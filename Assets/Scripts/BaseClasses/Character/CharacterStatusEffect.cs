using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class CharacterStatusEffect : MonoBehaviour
{
    public int duration = 0;
    public int difference = 0;
    public int basePotency = 0;
    public CharacterStat statAffected;
    CharacterGameEntity activeCharacter;


    public CharacterStatusEffect(int duration, int difference, int basePotency, CharacterStat stat, CharacterGameEntity characterAffected)
    {
        this.duration = duration;
        this.difference = difference;
        this.statAffected = stat;
        this.basePotency = basePotency;
        activeCharacter = characterAffected;

        characterAffected.characterData.AddToStat(stat, (int) difference, true);
        SetupLights(stat);
    }
    
    public void RevertChanges(bool ignoreBaseStat)
    {
        activeCharacter.characterData.AddToStat(statAffected, -difference, ignoreBaseStat);
        if (activeCharacter.characterData.statusEffects.Count == 1)
        {
            Destroy(activeCharacter.GetComponent<Light2D>());
            Destroy(activeCharacter.GetComponent<CyclicalLightIntensity>());
        }
        else
        {
            Light2D bonusLight = activeCharacter.GetComponent<Light2D>();
            switch (statAffected)
            {
                case CharacterStat.ATK :
                    bonusLight.color -= Color.red;
                    break;
                case CharacterStat.DEF :
                    bonusLight.color -= Color.blue;
                    break;
            }
        }
    }
    void SetupLights(CharacterStat stat)
    {
        Light2D bonusLight;
        if (activeCharacter.TryGetComponent<Light2D>(out Light2D presentLight))
            bonusLight = presentLight;
        else
            bonusLight = activeCharacter.gameObject.AddComponent<Light2D>();

        bonusLight.lightType = Light2D.LightType.Point;
        bonusLight.pointLightOuterRadius = 3;
        CyclicalLightIntensity intensity = activeCharacter.gameObject.AddComponent<CyclicalLightIntensity>();
        intensity.targetLight = bonusLight;
        intensity.startingIntensity = 2;
        intensity.minIntensity = 0.5f;

        switch (stat)
        {
            case CharacterStat.ATK :
                bonusLight.color += Color.red;
                break;
            case CharacterStat.DEF :
                bonusLight.color += Color.blue;
                break;
        }
    }
}

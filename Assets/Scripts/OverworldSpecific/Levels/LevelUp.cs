using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;
using TMPro;
public class LevelUp : MonoBehaviour
{
    [SerializeField] OverworldManager owManager;
    [SerializeField] UIListEntry listEntryPrefab;
    [SerializeField] UISelectable currentActiveUIElement;
    [SerializeField] UICursor cursor;
    [SerializeField] GameObject descriptor;
    [SerializeField] GameObject fade;

    [SerializeField] List<UISelectable> currentActiveUIElements = new List<UISelectable>();
    [SerializeField] List<Stats> listOfStats = new List<Stats>();

    PrimaryControls controls;
    TextMeshPro descriptorText;
    Timer startupTimer;

    void OnEnable()
    {
        startupTimer = new Timer(1);
        controls = new PrimaryControls();
        startupTimer.OnTimerEnd += ListSetup;
        controls.Enable();
        descriptorText = descriptor.GetComponentInChildren<TextMeshPro>();
    }
    void OnDisable()
    {
        ClearMenu();
        cursor.gameObject.SetActive(false);
    }
    void Update()
    {
        CharacterStat statToChange = 0;
        startupTimer.Tick(Time.deltaTime);
        if (controls.Battle.Direction.triggered)
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        
        cursor.cursorSelectable = currentActiveUIElement;
        string descriptionText = "";

        currentActiveUIElement.displayable.GetDisplayData(out Sprite[] sprites, out int[]ints, out string[] strings);

        switch(strings[0])
        {
            case ("Health") :
            {
                statToChange = CharacterStat.HP;
                descriptionText = "\n\nHEALTH : Increase your max HP! Higher health means you'll live longer!";
                break;
            }
            case ("Defense") :
            {
                statToChange = CharacterStat.DEF;
                descriptionText = "\n\nDEFENSE : Increase your defense! Protects you from incoming attacks!"; 
                break;
            }
            case ("Attack") :
            {
                statToChange = CharacterStat.ATK;
                descriptionText = "\n\nATTACK : Increase your attack power! Deal more damage to enemies, killing them sooner!";
                break;
            }
            case ("Speed") :
            {
                statToChange = CharacterStat.SPEED;
                descriptionText = "\n\nSPEED : Increase your speed! The faster you are means battles will start on your turn more often!";
                break;
            }
            case ("Stamina") :
            {
                statToChange = CharacterStat.STAMINA;
                descriptionText = "\n\nSTAMINA : Increase your max stamina! More stamina means you can combo enemies longer!";
                break;
            }
        }
        descriptorText.text = 
        "LEVEL UP! Choose a stat in which to upgrade! : " + descriptionText;

        if (controls.Battle.Primary.triggered)
        {
            CharacterBase playerData = owManager.playerCharacter.characterData;
            switch(statToChange)
            {
                case CharacterStat.ATK :
                {
                    playerData.baseATK += ints[0];
                    break;
                }
                case CharacterStat.DEF :
                {
                    playerData.baseDEF += ints[0];
                    break;
                }
                case CharacterStat.HP :
                {
                    playerData.baseHP += ints[0];
                    break;
                }
                case CharacterStat.SPEED :
                {
                    playerData.baseSPEED += ints[0];
                    break;
                }
                case CharacterStat.STAMINA :
                {
                    playerData.baseSTAMINA += ints[0];
                    break;
                }
            }
            GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.SELECT);
            
            playerData.ResetStatsToBaseValues();
            startupTimer = new Timer(1);
            startupTimer.OnTimerEnd += ReenableOverworld; 
            fade.GetComponent<Animator>().Play("fadeout");
            controls.Disable();

        }
    }

    void Navigate(int shift) // Navigate through the current list of active UI elements. 
    {
        int i;
        for (i = 0; i < currentActiveUIElements.Count; i++)
        {
            currentActiveUIElements[i].ShiftCycle(shift);
            if (currentActiveUIElements[i].cycle == 0) { currentActiveUIElement = currentActiveUIElements[i]; }
        }
        GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.NAVIGATE);
    }
    
    void ListSetup()
    {
        InstantiateList(new List<IDisplayable>(listOfStats), transform.position + new Vector3(-7.0f,5.0f,0));
        cursor.gameObject.SetActive(true);
        descriptor.SetActive(true);
    }
    void EnableCursor(Vector3 startPosition)
    {
        cursor.cursorSelectable = currentActiveUIElement;
        cursor.SetCursorInitialPosition(startPosition);
        cursor.gameObject.SetActive(true);
    }
    void InstantiateList(List<IDisplayable> ListEntries, Vector3 spawnerOrigin) // create a list of selectable items from the input list
    {
        if (listEntryPrefab == null)
            Debug.Log("No entry prefab loaded!");
        else
        {
            float yOffset = transform.localScale.y; // spawn each list element in the correct location
            for (int i = 0; i < ListEntries.Count; i++)
            {
                Vector3 target = spawnerOrigin;
                target.y -= (i * yOffset);
                UIListEntry objEntry = Instantiate(listEntryPrefab);
                objEntry.selectable.InitializeValues(i, ListEntries.Count, true, false, true);
                objEntry.selectable.InitializePositions(spawnerOrigin, target);
                objEntry.selectable.displayable = ListEntries[i];
                objEntry.SetDisplayData(ListEntries[i]);
                currentActiveUIElements.Add(objEntry.selectable);
            }
            currentActiveUIElement = currentActiveUIElements[0];
            EnableCursor(currentActiveUIElements[0].cursorTarget + spawnerOrigin);
        }
    }
    void ReenableOverworld()
    {
        owManager.gameObject.SetActive(true);
        owManager.ChildObjects[1].GetComponent<Animator>().Play("fadein");
        owManager.nextLevel();
        Debug.Log("back to overworld");
        gameObject.SetActive(false);
    }
    void ClearMenu()
    {
        if (currentActiveUIElements.Count > 0)
        {
            for (int i = 0; i < currentActiveUIElements.Count; i++)
            {
                if (currentActiveUIElements[i].isDestroyable)
                    Destroy(currentActiveUIElements[i].gameObject);
                if (currentActiveUIElements[i].canBeDisabled)
                    currentActiveUIElements[i].gameObject.SetActive(false);
            }
        }

        currentActiveUIElements.Clear();
        descriptor.SetActive(false);
    }
}

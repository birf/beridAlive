using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeriUtils.Core;

public class TitleScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GenericSelection> options = new List<GenericSelection>();
    [SerializeField] List<UISelectable> currentActiveUIElements = new List<UISelectable>();
    [SerializeField] UISelectable currentActiveUIElement;
    [SerializeField] UICursor cursor;
    [SerializeField] UIListEntry listEntryPrefab;
    [SerializeField] GameObject descriptor;
    [SerializeField] GameObject titleLogo;
    [SerializeField] Animator fade;
    [SerializeField] AudioManager am;
    PrimaryControls controls;

    Timer killTimer = new Timer(1.5f);
    int substate = 0;
    bool loadNextScene = false;
    bool quitGame = false;

    string currentSelection = "";

    void Start()
    {
        controls = new PrimaryControls();
        controls.Enable();
        am = GetComponent<AudioManager>();
        InstantiateList(new List<IDisplayable>(options),transform.position - Vector3.up * 2f);
        EnableCursor(transform.position - Vector3.up * 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (substate == 0)
        {
            SelectOption(); 
        }
        else if (substate == 1)
        {
            DisplayDescription();
        }
        else if (substate == 2)
        {
            killTimer.Tick(Time.deltaTime);
        }
        titleLogo.transform.position = Vector3.Lerp(titleLogo.transform.position,new Vector3(0,1.75f,0),Time.deltaTime * 10f);
    }
    void SelectOption()
    {
        if (controls.Battle.Direction.triggered)
            Navigate((int)controls.Battle.Direction.ReadValue<Vector2>().y);
        
        currentActiveUIElement.displayable.GetDisplayData(out Sprite[] sprites, out int[]ints, out string[] strings);
        currentSelection = strings[0];
        cursor.cursorSelectable = currentActiveUIElement;

        if (controls.Battle.Primary.triggered)
        {
            am.PlayTrack(AUDIOCLIPS.SELECT);
            switch(currentSelection)
            {
                case "Play" :
                    loadNextScene = true;
                    fade.Play("fadeout");
                    killTimer.OnTimerEnd += StartGame;
                    substate = 2;
                    break;
                case "Story" :
                    substate = 1;
                    break;
                case "Quit" :
                    quitGame = true;
                    substate = 2;
                    fade.Play("fadeout");
                    killTimer.OnTimerEnd += QuitGame;
                    break;
            }
        }
    }
    void DisplayDescription()
    {
        descriptor.SetActive(true);
        if (controls.Battle.Secondary.triggered)
        {
            substate = 0;
            GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.DESELECT);
            descriptor.SetActive(false);
            return;
        }
    }
    void StartGame()
    {
        SceneManager.LoadScene("MainGame");    
    }
    void QuitGame()
    {
        Application.Quit();
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
    void EnableCursor(Vector3 startPosition)
    {
        cursor.cursorSelectable = currentActiveUIElement;
        cursor.SetCursorInitialPosition(startPosition);
        cursor.gameObject.SetActive(true);
    }
}

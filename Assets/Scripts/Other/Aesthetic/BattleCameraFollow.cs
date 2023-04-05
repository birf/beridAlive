using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraFollow : MonoBehaviour
{
    public float maxZoom = 6.0f;
    public float minZoom = 6.7f;
    [SerializeField] Camera cameraObj;
    [SerializeField] BattleManager battleManager;
    [SerializeField] [Range(0f,1f)] float slider = 0;
    CharacterGameBattleEntity activeChar;
    CharacterGameBattleEntity targetChar;


    void Awake()
    {
        battleManager = (BattleManager)CentralManager.GetStateManager();
        cameraObj = GetComponent<Camera>();
    }
    void OnEnable()
    {
        transform.position = new Vector3(0,0,-10f);
    }
    void FixedUpdate()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        // only move camera when the game state calls for it
        if (BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.PLAYERATTACK
            || BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.ENEMYTURN
            || BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.ANALYSIS
            || BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.WAIT
            || BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.LOSE)
        {
            activeChar = battleManager.currentActiveCharacter;
            targetChar = battleManager.currentTargetCharacter;

            
            float distanceRatio = 1;
            Vector3 cameraPos = new Vector3(0,0,-10);
            if (!activeChar || !targetChar)
            {
                transform.position = Vector3.Lerp(transform.position,new Vector3(0,0,-10),Time.deltaTime * 2.0f);
                cameraObj.orthographicSize = Mathf.Lerp(cameraObj.orthographicSize, minZoom, Time.deltaTime);
                return;
            }
            
            if (targetChar != activeChar)
            {
                distanceRatio = 
                    Vector2.Distance(activeChar.transform.position,targetChar.transform.position) /
                    Vector2.Distance(activeChar.characterBattlePhysics.startPosition, targetChar.characterBattlePhysics.startPosition);
                cameraPos = new Vector3(activeChar.transform.position.x + targetChar.transform.position.x, 0, -10);
            }
            
            cameraObj.orthographicSize = Mathf.Lerp(minZoom, maxZoom, 1f - distanceRatio);
            
            Mathf.Clamp(cameraPos.x,-10.0f,10.0f);
            Mathf.Clamp(transform.position.x,-10.0f,10.0f);

            transform.position = Vector3.Lerp(transform.position,cameraPos,Time.deltaTime);

        }
        else
        {
            
            transform.position = Vector3.Lerp(transform.position,new Vector3(0,0,-10),Time.deltaTime * 2.0f);
            cameraObj.orthographicSize = Mathf.Lerp(cameraObj.orthographicSize, minZoom, Time.deltaTime);
        }
    }
    public void OverrideTargetCharacter(CharacterGameBattleEntity newTarget)
    {
        targetChar = newTarget;
    }
}

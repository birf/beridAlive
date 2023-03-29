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

    void Awake()
    {
        battleManager = (BattleManager)CentralManager.GetStateManager();
        cameraObj = GetComponent<Camera>();
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
            || BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.WIN
            || BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.LOSE)
        {
            CharacterGameBattleEntity activeChar = battleManager.currentActiveCharacter;
            CharacterGameBattleEntity targetChar = battleManager.currentTargetCharacter;
            if (!activeChar || !targetChar)
                return;
            
            float distanceRatio = 0;
            if (targetChar != activeChar)
                distanceRatio = 
                    Vector2.Distance(activeChar.transform.position,targetChar.transform.position) /
                    Vector2.Distance(activeChar.characterBattlePhysics.startPosition, targetChar.characterBattlePhysics.startPosition);
            
            cameraObj.orthographicSize = Mathf.Lerp(minZoom, maxZoom, 1f - distanceRatio);
            
            Vector3 cameraPos = new Vector3(activeChar.transform.position.x + targetChar.transform.position.x, 0, -10);
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
}

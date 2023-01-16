// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TestListSpawner : MonoBehaviour
// {
//     public List<ItemData> itemList = new List<ItemData>();
//     public UIListEntry entryPrefab; 
//     void Start()
//     {
//         Vector3 t = transform.position;
//         if (entryPrefab == null)
//             Debug.Log("No entry prefab loaded!");
//         else
//         {
//             float yOffset = transform.localScale.y;
//             for (int i = 0; i < itemList.Count; i++)
//             {
//                 GameObject obj = entryPrefab.gameObject;
//                 UIListEntry entry = obj.GetComponent<UIListEntry>();
//                 entry.SetDisplayData(itemList[i]);
//                 entry.selectable.InitializeValues(i,itemList.Count,true,false,true,true);
//                 if (i == 0)
//                     entry.selectable.cursorIsHovering = true;
//                 entry.targetPosition = new Vector3(t.x,t.y - (i * yOffset),0);
//                 Instantiate(obj);
//             }
//         }
//     }
// }

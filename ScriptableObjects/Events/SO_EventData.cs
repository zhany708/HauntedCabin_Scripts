using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "newEventData", menuName = "Data/Event Data/Event")]
public class SO_EventData : ScriptableObject
{
    public string EventName;
    public GameObject EventPrefab;      //事件预制件
}

using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "newEventData", menuName = "Data/Event Data/Event")]
public class SO_EventData : ScriptableObject
{
    //特性相关
    [Header("Feature")]
    public string EventName;
    public GameObject EventPrefab;       //事件预制件


    //音效相关
    [Header("Audio")]
    public List<string> AudioClipNames;  //事件的音效名，用于加载
    public float AudioVolume = 1f;       //音效的音量

    /*
    //选项相关
    [Header("Option")]
    public List<string> OptionResults;    //选项的结果文本
    */
}

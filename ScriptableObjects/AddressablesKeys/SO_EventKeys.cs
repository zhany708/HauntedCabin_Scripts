using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newEventKeys", menuName = "Data/Event Data/Event Keys")]
public class SO_EventKeys : ScriptableObject
{
    //事件需要用列表，否则不方便随机生成

    //预兆事件
    [Header("Evil Event")]
    public List<string> EvilEventKeys;

    //普通事件
    [Header("Normal Event")]
    public List<string> NormalEventKeys;
}
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "newEventData", menuName = "Data/Event Data/Event")]
public class SO_EventData : ScriptableObject
{
    public string EventName;
    public GameObject EventPrefab;      //�¼�Ԥ�Ƽ�

    public List<string> AudioClipNames;         //�¼�����Ч�������ڼ���
    public float AudioVolume = 1f;       //��Ч������
}

using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "newEventData", menuName = "Data/Event Data/Event")]
public class SO_EventData : ScriptableObject
{
    //�������
    [Header("Feature")]
    public string EventName;
    public GameObject EventPrefab;       //�¼�Ԥ�Ƽ�


    //��Ч���
    [Header("Audio")]
    public List<string> AudioClipNames;  //�¼�����Ч�������ڼ���
    public float AudioVolume = 1f;       //��Ч������

    /*
    //ѡ�����
    [Header("Option")]
    public List<string> OptionResults;    //ѡ��Ľ���ı�
    */
}

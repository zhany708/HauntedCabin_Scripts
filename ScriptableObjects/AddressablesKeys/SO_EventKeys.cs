using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newEventKeys", menuName = "Data/Event Data/Event Keys")]
public class SO_EventKeys : ScriptableObject
{
    //�¼���Ҫ���б����򲻷����������

    //Ԥ���¼�
    [Header("Evil Event")]
    public List<string> EvilEventKeys;

    //��ͨ�¼�
    [Header("Normal Event")]
    public List<string> NormalEventKeys;
}
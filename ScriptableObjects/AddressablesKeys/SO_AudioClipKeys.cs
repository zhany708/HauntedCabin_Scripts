using UnityEngine;


[CreateAssetMenu(fileName = "newAudioClipKeys", menuName = "Data/Audio Data/Clip Keys")]
public class SO_AudioClipKeys : ScriptableObject
{
    //BGM
    [Header("BGM Music")]



    //ĳЩ��Ч��Ϊ�ڴ���������ScriptableObjectʱ�Ѿ�������Ч�����ˣ���˲���������ȡ���֣�

    //������Ч
    [Header("Weapon Audio")]
    public string ShotgunKey;


    //�¼���Ч
    [Header("Event Audio")]
    public string PhoneRingKey;
    public string AnswerPhoneKey;
}
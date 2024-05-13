using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class SO_PlayerData : ScriptableObject      //����ͨ���˽ű�����Asset
{
    [Header("Movement Values")]
    public float MovementVelocity = 2f;     //������ٵ�Ĭ��ֵΪ2

    [Header("Status Values")]
    public float MaxHealth = 10f;   //�������ֵ
    public float Strength = 5f;     //�������ĸ����Ե�Ĭ��ֵ��Ϊ5��
    public float Speed = 5f;        //�ٶ�
    public float Sanity = 5f;       //��־
    public float Knowledge = 5f;    //֪ʶ
    public float Defense = 2f;      //����

    [Header("Hit Values")]
    public float HitResistance = 1f;
}
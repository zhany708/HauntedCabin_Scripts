using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class SO_PlayerData : ScriptableObject      //����ͨ���˽ű�����Asset
{
    [Header("Movement Values")]
    public float MovementVelocity = 2f;

    [Header("Status Values")]
    public float MaxHealth = 10f;   //�������ֵ
    public float Strength = 1f;     //����
    public float Speed = 1f;        //�ٶ�
    public float Sanity = 1f;       //��־
    public float Knowledge = 1f;    //֪ʶ
    public float Defense = 2f;      //����

    [Header("Hit Values")]
    public float HitResistance = 1f;
}
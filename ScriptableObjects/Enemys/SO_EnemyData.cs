using UnityEngine;


[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Base Data")]
public class SO_EnemyData : ScriptableObject
{
    [Header("Movement Vlaues")]
    public float MoveSpeed = 0;
    public float ChaseSpeed = 0;
    public float IdleDuration = 0;      //待机时长
    public float StoppingDistance = 0;  //敌人与玩家的最小距离

    [Header("Attack Values")]
    public LayerMask TargetLayer = 0;
    public float AttackArea = 0;        //攻击检测圆的半径参数
    public float AttackInterval = 0;    //攻击间隔

    [Header("Status Values")]
    public float MaxHealth = 0;
    public float Defense = 0;

    [Header("Hit Values")]
    public float HitResistance = 0;
    public float HitInterval = 0;   //无敌时间
}
using UnityEngine;


[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Base Data")]
public class SO_EnemyData : ScriptableObject
{
    [Header("移动相关")]
    public float MoveSpeed = 0;
    public float ChaseSpeed = 0;
    public float IdleDuration = 0;          //待机时长
    public float StoppingDistance = 0;      //敌人与玩家的最小距离

    [Header("攻击相关")]
    public LayerMask PlayerLayer = 0;
    public float AttackArea = 0;            //攻击检测圆的半径参数
    public float AttackInterval = 0;        //攻击间隔
    public float AttackSpeed = 0;           //攻击速度（远程攻击的话就是飞行物的速度）
    public float DamageAmount = 0;          //攻击基础伤害
    public float KnockbackStrength = 0;     //击退力度

    [Header("状态相关")]
    public float MaxHealth = 0;
    public float Defense = 0;

    [Header("受击相关")]
    public float HitResistance = 0;
    public float HitInterval = 0;           //无敌时间

    [Header("剧本相关")]
    public LayerMask AltarLayer = 0;
}
using System;
using UnityEngine;
using ZhangYu.Utilities;



#region Parameters
[Serializable]      //让编辑器序列化这个类
public class EnemyParameter_DefenseWar : EnemyParameter     //新的敌人Parameter
{
    //攻击相关
    public Transform AltarTarget;     //祷告石的坐标
}
#endregion




//用于保卫战的敌人脚本，主要少了几个状态，以及优先攻击祷告石的设定
public class Enemy_DefenseWar : Enemy
{
    #region FSM States
    public new EnemyChaseState_DefenseWar ChaseState { get; private set; }
    public new EnemyAttackState_DefenseWar AttackState { get; protected set; }
    public new EnemyHitState_DefenseWar HitState { get; protected set; }
    public new EnemyDeathState_DefenseWar DeathState { get; protected set; }


    //检查m_Stats是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    public Stats Stats => m_Stats ? m_Stats : Core.GetCoreComponent(ref m_Stats);       
    private Stats m_Stats;
    #endregion


    #region Variables
    public EnemyParameter_DefenseWar Parameter_DefenseWar;
    #endregion


    #region Unity Callback Functions
    protected override void Awake()    //最早实施的函数（只实施一次）
    {  
        base.Awake();

        ChaseState = new EnemyChaseState_DefenseWar(this, StateMachine, enemyData, "Idle");
        AttackState = new EnemyAttackState_DefenseWar(this, StateMachine, enemyData, "Attack");
        HitState = new EnemyHitState_DefenseWar(this, StateMachine, enemyData, "Hit");
        DeathState = new EnemyDeathState_DefenseWar(this, StateMachine, enemyData, "Death");
    }

    protected override void Start()      //只在第一帧运行前运行一次这个函数
    {
        StateMachine.Initialize(ChaseState);     //初始化状态为追击
    }

    protected override void OnEnable()       //每次重新激活时都会运行这个函数
    {
        AttackTimer = new Timer(enemyData.AttackInterval);      //用攻击间隔初始化计时器
        EnemyFlip = new Flip(transform);


        //激活前提前把祷告石的位置赋予敌人
        if (Parameter_DefenseWar.AltarTarget == null)
        {
            Parameter_DefenseWar.AltarTarget = FindAnyObjectByType<Altar>().gameObject.transform;
        }


        CanAttack = true;   //重新激活时将可攻击设置为true
        AttackTimer.OnTimerDone += SetCanAttackTrue;        //连接事件，使敌人可以重新攻击
        Stats.OnHalfHealth += ChangeAttackInterval;         //连接事件，使敌人的攻击间隔缩短


        if (isReactivate)     //敌人重新激活后才会在这里初始化追击状态，否则第一次生成时如果在这初始化会因为脚本的实施顺序出现null错误
        {
            StateMachine.Initialize(ChaseState);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Stats.OnHalfHealth -= ChangeAttackInterval;   
    }
    #endregion


    #region Main Functions
    private void ChangeAttackInterval()     //改变攻击间隔
    {
        //根据当前血量百分比缩短攻击间隔（比如当前20%的血量就对应着原本攻击间隔的20%的时长）
        AttackTimer.SetDuration(enemyData.AttackInterval * Stats.GetCurrentHelathRate() );
    }
    #endregion


    #region Trigger Detections
    //各种物理检测
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Altar") && Parameter_DefenseWar.AltarTarget == null)
        {
            Parameter_DefenseWar.AltarTarget = other.transform;     //储存祷告石的位置信息
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Parameter_DefenseWar.PlayerTarget = null;     //玩家退出范围时清空
        }
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Parameter_DefenseWar.AttackPoint.position, enemyData.AttackArea);    //设置攻击范围的圆心和半径
    }
    #endregion


    #region Getters

    #endregion


    #region Setters

    #endregion
}
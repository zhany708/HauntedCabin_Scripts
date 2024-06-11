using System;
using UnityEngine;
using ZhangYu.Utilities;


/* 新的敌人Parameter
#region Parameters
[Serializable]      //让编辑器序列化这个类
public class EnemyParameter
{
    //基础信息
    //public Transform[] PatrolPoints;    //巡逻范围

    //攻击相关
    public Transform PlayerTarget;          //玩家的坐标
    public Transform AltarTarget;     //祷告石的坐标
    //public Transform[] ChasePoints;     //追击范围
    public Transform AttackPoint;     //攻击范围的圆心位置
}
#endregion
*/



//用于保卫战的敌人脚本，主要少了几个状态，以及优先攻击祷告石的设定
public class Enemy_DefenseWar : Enemy
{
    #region FSM States
    /*
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    */

    public new EnemyChaseState_DefenseWar ChaseState { get; private set; }

    /*
    public EnemyAttackState AttackState { get; protected set; }
    public EnemyHitState HitState { get; private set; }
    public EnemyDeathState DeathState { get; protected set;}
    #endregion

    #region Components
    public EnemyParameter Parameter;
    public Core Core { get; private set; }

    public Movement Movement => m_Movement ? m_Movement : Core.GetCoreComponent(ref m_Movement);   //检查m_Movement是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    private Movement m_Movement;

    public Combat Combat => m_Combat ? m_Combat : Core.GetCoreComponent(ref m_Combat);             //检查m_Combat是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    private Combat m_Combat;
    */

    public Stats Stats => m_Stats ? m_Stats : Core.GetCoreComponent(ref m_Stats);                  //检查m_Stats是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    private Stats m_Stats;

    /*  基础调用核心组件的方法
    public EnemyDeath Death
    {
        get
        {
            if (m_Death) { return m_Death; }      //检查组件是否为空
            m_Death = Core.GetCoreComponent<EnemyDeath>();
            return m_Death;
        }
    }
    private EnemyDeath m_Death;
    */

    /*
    public DoorController DoorController { get; private set; }      //用于敌人死亡状态中增加DoorController脚本中敌人死亡计数的整数

    public Timer AttackTimer { get; private set; }
    //public RandomPosition PatrolRandomPos { get; private set; }
    public Flip EnemyFlip { get; private set; }



    [SerializeField]
    protected SO_EnemyData enemyData;
    */
    #endregion


    #region Variables
    /*
    //public bool CanAttack { get; private set; } = true;        //用于攻击间隔

    //float m_LastHitTime;        //上次受击时间
    protected bool isReactivate = false;    //判断敌人是否为重新激活
    */
    #endregion


    #region Unity Callback Functions
    protected override void Awake()    //最早实施的函数（只实施一次）
    {  
        base.Awake();

        ChaseState = new EnemyChaseState_DefenseWar(this, StateMachine, enemyData, "Idle");
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
        if (Parameter.AltarTarget == null)
        {
            Parameter.AltarTarget = Altar.Instance.gameObject.transform;
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
        if (other.CompareTag("Altar") && Parameter.AltarTarget == null)
        {
            Parameter.AltarTarget = other.transform;     //储存祷告石的位置信息
        }
    }
    #endregion


    #region Getters

    #endregion


    #region Setters
    
    #endregion
}
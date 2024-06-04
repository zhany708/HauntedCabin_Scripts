using System;
using UnityEngine;
using ZhangYu.Utilities;



#region Parameters
[Serializable]      //让编辑器序列化这个类
public class EnemyParameter
{
    //基础信息
    public Transform[] PatrolPoints;    //巡逻范围

    //攻击相关
    public Transform Target;     //玩家的坐标
    //public Transform[] ChasePoints;     //追击范围
    public Transform AttackPoint;   //攻击范围的圆心位置
}
#endregion





public class Enemy : MonoBehaviour
{
    #region FSM States
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; protected set; }
    public EnemyHitState HitState { get; private set; }
    public EnemyDeathState DeathState { get; protected set;}
    #endregion

    #region Components
    public EnemyParameter Parameter;
    public Core Core { get; private set; }

    public Movement Movement => m_Movement ? m_Movement : Core.GetCoreComponent(ref m_Movement);   //检查m_Movement是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    private Movement m_Movement;

    public Combat Combat => m_Combat ? m_Combat : Core.GetCoreComponent(ref m_Combat);   //检查m_Movement是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    private Combat m_Combat;

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

    public DoorController DoorController { get; private set; }      //用于敌人死亡状态中增加DoorController脚本中敌人死亡计数的整数

    public Timer AttackTimer { get; private set; }
    public RandomPosition PatrolRandomPos { get; private set; }
    public Flip EnemyFlip { get; private set; }



    [SerializeField]
    protected SO_EnemyData enemyData;
    #endregion

    #region Variables
    public bool CanAttack { get; private set; } = true;        //用于攻击间隔

    //float m_LastHitTime;        //上次受击时间
    bool m_IsReactivate = false;    //判断敌人是否为重新激活
    #endregion

    #region Unity Callback Functions
    private void Awake()    //最早实施的函数（只实施一次）
    {  
        Core = GetComponentInChildren<Core>();      //从子物体那调用Core脚本
        Core.SetParameters(enemyData.MaxHealth, enemyData.Defense, enemyData.HitResistance);    //设置参数

        StateMachine = new EnemyStateMachine();

        //初始化各状态
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "Idle");
        PatrolState = new EnemyPatrolState(this, StateMachine, enemyData, "Idle");
        ChaseState = new EnemyChaseState(this, StateMachine, enemyData, "Idle");
        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "Attack");
        HitState = new EnemyHitState(this, StateMachine, enemyData, "Hit");
        DeathState = new EnemyDeathState(this, StateMachine, enemyData, "Death");       
    }

    protected virtual void Start()      //只在第一帧运行前运行一次这个函数
    {
        StateMachine.Initialize(IdleState);     //初始化状态为闲置
    }

    private void Update()
    {
        Core.LogicUpdate();     //获取当前速度

        StateMachine.CurrentState.LogicUpdate();

        AttackTimer.Tick();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    protected virtual void OnEnable()       //每次重新激活时都会运行这个函数
    {
        AttackTimer = new Timer(enemyData.AttackInterval);      //用攻击间隔初始化计时器

        //根据父物体的坐标初始化随机生成坐标脚本（transform.localPosition返回的永远是相对于父物体的坐标），使用跟物体的世界坐标进行计算（因为只有跟物体的坐标在这之前赋过值了）
        Vector2 leftDownPos = Parameter.PatrolPoints[0].transform.localPosition + transform.parent.position;
        Vector2 rightTopPos = Parameter.PatrolPoints[1].transform.localPosition + transform.parent.position;

        PatrolRandomPos = new RandomPosition(leftDownPos, rightTopPos, 1f);     
        //Debug.Log("The LeftDown point of PatrolRandomPos is " + PatrolRandomPos.GetLeftDownPos() + ", and the RightTop point is " + PatrolRandomPos.GetRightTopPos() );
        

        EnemyFlip = new Flip(transform);



        foreach (Transform child in transform.parent)    //在场景中取消激活所有巡逻点
        {
            foreach (Transform child2 in child)     //在敌人的父物体中检索每一个子物体的子物体
            {
                if (child2.CompareTag("PatrolPoint"))
                {
                    child2.gameObject.SetActive(false);  
                }
            }
        }


        CanAttack = true;   //重新激活时将可攻击设置为true
        AttackTimer.OnTimerDone += SetCanAttackTrue;        //触发事件，使敌人可以重新攻击


        if (m_IsReactivate)     //敌人重新激活后才会在这里初始化闲置状态，否则第一次生成时如果在这初始化会因为脚本的实施顺序出现null错误
        {
            StateMachine.Initialize(IdleState);
        }
    }

    private void OnDisable()
    {
        Movement.SetVelocityZero();     //取消激活后将刚体速度重置，防止报错

        Combat.SetIsHit(false);  //敌人死亡后设置Combat中的受击布尔为false，以便及时进入死亡状态，且防止重新激活后直接进入受击状态

        m_IsReactivate = true;      //表示已经重新激活

        AttackTimer.OnTimerDone -= SetCanAttackTrue;
    }
    #endregion

    #region Main Functions
    /*
    //检测玩家是否超出追击范围
    public bool CheckOutside()
    {
        float minX = Mathf.Min(Parameter.ChasePoints[0].position.x, Parameter.ChasePoints[1].position.x);
        float minY = Mathf.Min(Parameter.ChasePoints[0].position.y, Parameter.ChasePoints[1].position.y);
        float maxX = Mathf.Max(Parameter.ChasePoints[0].position.x, Parameter.ChasePoints[1].position.x);
        float maxY = Mathf.Max(Parameter.ChasePoints[0].position.y, Parameter.ChasePoints[1].position.y);

        return Parameter.Target.position.x < minX || Parameter.Target.position.x > maxX || Parameter.Target.position.y < minY || Parameter.Target.position.y > maxY;
    }
    */
    private void SetCanAttackTrue()     //用于Action，由于不能传参数，因此不能用下面Setters里的函数
    {
        CanAttack = true;
    }
    #endregion

    #region Animation Event Functions
    private void DeathLogicForAnimation()      //用于动画事件，摧毁物体
    {
        if (transform.parent != null)
        {
            //检查游戏是否已经结束，如果结束则无需再进行计数
            if (DoorController != null && !EnvironmentManager.Instance.IsGameOver)
            {
                DoorController.IncrementEnemyCount();     //增加敌人计数器的计数
            }
            
            EnemyPool.Instance.PushObject(transform.parent.gameObject);      //将敌人的父物体放回池中，也将放回父物体的所有子物体
        }
    }
    #endregion

    #region Trigger Detections
    //各种物理检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Parameter.Target = other.transform;     //储存玩家的位置信息
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Parameter.Target = null;     //玩家退出范围时清空
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Parameter.AttackPoint.position, enemyData.AttackArea);    //设置攻击范围的圆心和半径
    }
    #endregion

    #region Getters
    //获取成员变量
    /*
    public float GetLastHitTime()
    {
        return m_LastHitTime;
    }
    */
    #endregion

    #region Setters
    //设置成员变量
    public void SetDoorController(DoorController door)
    {
        DoorController = door;
    }

    public void SetCanAttack(bool isTrue)
    {
        CanAttack = isTrue;
    }

    /*
    public void SetLastHitTime(float currentTime)
    {
        m_LastHitTime = currentTime;
    }
    */

    public void ResetLocalPos()
    {
        //将此脚本绑定的物体的相对于父物体的坐标清零
        transform.localPosition = Vector2.zero;
    }
    #endregion
}
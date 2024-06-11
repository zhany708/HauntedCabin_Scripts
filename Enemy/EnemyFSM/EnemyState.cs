using UnityEngine;


public class EnemyState
{
    protected Core core;
    protected Movement enemyMovement
    {
        get
        {
            if (m_Movement) { return m_Movement; }      //检查组件是否为空
            m_Movement = core.GetCoreComponent<Movement>();
            return m_Movement;
        }
    }
    private Movement m_Movement;


    protected Combat enemyCombat
    {
        get
        {
            if (m_Combat) { return m_Combat; }
            m_Combat = core.GetCoreComponent<Combat>();
            return m_Combat;
        }
    }
    private Combat m_Combat;

    protected Stats enemyStats
    {
        get
        {
            if (m_Stats) { return m_Stats; }
            m_Stats = core.GetCoreComponent<Stats>();
            return m_Stats;
        }
    }
    private Stats m_Stats;



    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected SO_EnemyData enemyData;

    protected bool isHit = false;


    protected string animationBoolName;     //告诉动画器应该播放哪个动画










    public EnemyState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.enemyData = enemyData;
        animationBoolName = animBoolName;
        core = enemy.Core;

        if (!core)
        {
            Debug.LogError("Core is missing in the EnemyState!");
        }
    }


    public virtual void Enter()
    {
        if (!enemyMovement || !enemyCombat || !enemyStats)
        {
            Debug.Log("Something is wrong in the EnemyState!");
        }


        core.Animator.SetBool(animationBoolName, true);     //播放状态的动画
        
        //Debug.Log(m_AnimationBoolName);
    }

    public virtual void LogicUpdate()
    {
        //先判断是否死亡（不能放在受击状态中判断，因为敌人攻击时不会进入受击状态）
        if (enemyStats.CurrentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.DeathState);
        }

        else if (enemyCombat.IsHit && !isHit)
        {
            stateMachine.ChangeState(enemy.HitState);
        }
    }

    public virtual void PhysicsUpdate() 
    {
        if (isHit)   //受击状态中
        {
            float amount = 0.3f;
            amount += Time.deltaTime;

            enemyMovement.ReduceVelocity(amount);    //持续减少移动速度
        }

        else
        {
            enemyMovement.SetVelocityZero();     //受击结束后使敌人停止移动，也防止玩家碰撞敌人后敌人持续移动
        }
    }

    public virtual void Exit()
    {
        enemy.Core.Animator.SetBool(animationBoolName, false);        //设置当前状态布尔为false以进入下个状态
    }
}
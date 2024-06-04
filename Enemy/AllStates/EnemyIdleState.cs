using UnityEngine;


public class EnemyIdleState : EnemyState
{
    float m_IdleTimer;      //巡逻（原地停留）时间

    public EnemyIdleState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        enemyMovement.SetVelocityZero();       //将速度归零
    }

    public override void LogicUpdate()
    {
        m_IdleTimer += Time.deltaTime;

        base.LogicUpdate();

        if (enemy.Parameter.Target != null)      //&& !enemy.CheckOutside())
        {
            stateMachine.ChangeState(enemy.ChaseState);     //如果检测到玩家则切换为追击状态（如果有反应动画可以先切换为反应状态。然后再反应动画播完后切换成追击状态）
        }


        else if (m_IdleTimer >= enemyData.IdleDuration)    //检测是否该进入巡逻状态
        {
            stateMachine.ChangeState(enemy.PatrolState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        m_IdleTimer = 0;
    }
}

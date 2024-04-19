using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    Vector2 m_RandomPosition;       //随机坐标
    float m_PatrolTimer;

    public EnemyPatrolState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        //生成随机坐标
        m_RandomPosition = enemy.PatrolRandomPos.GenerateSingleRandomPos();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        m_PatrolTimer += Time.deltaTime;

        enemy.EnemyFlip.FlipX( enemyMovement.GetFlipNum(m_RandomPosition, enemy.transform.position) );  //朝向巡逻点的方向

        if (enemy.Parameter.Target != null)     // && !enemy.CheckOutside())
        {
            stateMachine.ChangeState(enemy.ChaseState);     //巡逻时如果检测到玩家则切换为追击状态
        }

        else if (Vector2.Distance(enemy.transform.position, m_RandomPosition) < 0.1f)     //当距离目标巡逻点足够近时
        {
            stateMachine.ChangeState(enemy.IdleState);
        }

        else if (m_PatrolTimer >= 5f)
        {
            stateMachine.ChangeState(enemy.IdleState);      //如果5秒后敌人仍没有到达巡逻点（卡住了），则强制转换成闲置状态
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (m_RandomPosition != null)
        {
            //移动到目标点
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, m_RandomPosition, enemyData.MoveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        m_PatrolTimer = 0f;     //退出时将计时器清零
    }
}

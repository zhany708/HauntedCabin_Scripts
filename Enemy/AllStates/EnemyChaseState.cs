using UnityEngine;
using ZhangYu.Utilities;


public class EnemyChaseState : EnemyState
{
    float m_DistanceToPlayer;

    public EnemyChaseState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void LogicUpdate()
    {
        if (enemy.Parameter.Target != null)
        {
            enemy.EnemyFlip.FlipX( enemy.Movement.GetFlipNum(enemy.Parameter.Target.position, enemy.transform.position) );    //使怪物朝向玩家

            m_DistanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.Parameter.Target.position);       //计算敌人与玩家的距离
        }

        base.LogicUpdate();

        if (enemy.Parameter.Target == null || enemy.CheckOutside())
        {
            stateMachine.ChangeState(enemy.IdleState);      //丢失目标或者超出追击范围时切换到待机状态
        }

        //检测攻击范围：第一个参数为圆心位置，第二个为半径，第三个为目标图层.玩家处于攻击范围且攻击间隔结束则进入攻击状态
        else if (Physics2D.OverlapCircle(enemy.Parameter.AttackPoint.position, enemyData.AttackArea, enemyData.TargetLayer) && enemy.CanAttack)
        {
            stateMachine.ChangeState(enemy.AttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.Parameter.Target && m_DistanceToPlayer > enemyData.StoppingDistance)     //有玩家坐标且与玩家距离大于最小距离时持续追击玩家
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.Parameter.Target.position, enemyData.ChaseSpeed * Time.deltaTime);
        }
    }
}

using UnityEngine;


public class EnemyHitState : EnemyState
{
    public EnemyHitState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        isHit = true;
        //enemy.SetLastHitTime(Time.time);     //设置当前时间为上次受击时间
    }

    public override void LogicUpdate()
    {
        if (enemyStats.GetCurrentHealth() == 0)
        {
            stateMachine.ChangeState(enemy.DeathState);
        }

        else if (core.AnimatorInfo.IsName("Hit"))
        {
            if (core.AnimatorInfo.normalizedTime >= 0.95f)
            {
                enemy.Parameter.Target = GameObject.FindWithTag("Player").transform;        //寻找有Player标签的物件坐标
                stateMachine.ChangeState(enemy.ChaseState);
            }

            else if (core.AnimatorInfo.normalizedTime >= 0.5f)
            {
                enemyMovement.SetVelocityZero();     //动画播到50%时停止移动
            }        
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemyCombat.SetIsHit(false);
        isHit = false;
    }
}

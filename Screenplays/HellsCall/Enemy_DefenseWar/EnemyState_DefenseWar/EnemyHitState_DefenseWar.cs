using UnityEngine;


public class EnemyHitState_DefenseWar : EnemyState_DefenseWar
{
    public EnemyHitState_DefenseWar(Enemy_DefenseWar enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
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
        //受击结束后储存玩家的坐标，并进入追击状态
        if (core.AnimatorInfo.IsName("Hit"))
        {
            if (core.AnimatorInfo.normalizedTime >= 0.95f)
            {
                enemy.Parameter_DefenseWar.PlayerTarget = GameObject.FindWithTag("Player").transform;        //寻找有Player标签的物件坐标
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
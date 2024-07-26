using UnityEngine;



public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        enemy.AttackTimer.StopTimer();     //攻击期间停止计时

        //Debug.Log("Enemy entered AttackState");
    }

    public override void LogicUpdate()      //重写逻辑函数，使敌人不会在攻击状态中进入受击状态
    {
        if (core.AnimatorInfo.IsName("Attack") && core.AnimatorInfo.normalizedTime >= 0.95f)
        {
            stateMachine.ChangeState(enemy.ChaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemyCombat.SetIsHit(false);     //如果没有这行代码，那么敌人在攻击状态中被击中的话，会强制在攻击状态结束后进入受击状态
        enemy.SetCanAttack(false);
        enemy.AttackTimer.StartTimer();     //攻击结束后开始计时
    }
}

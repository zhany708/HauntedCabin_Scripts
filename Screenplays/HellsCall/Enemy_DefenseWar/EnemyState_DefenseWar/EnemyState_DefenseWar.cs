using UnityEngine;


public class EnemyState_DefenseWar : EnemyState
{
    protected new Enemy_DefenseWar enemy;






    public EnemyState_DefenseWar(Enemy_DefenseWar enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.enemyData = enemyData;
        animationBoolName = animBoolName;
        core = enemy.Core;

        if (!core)
        {
            Debug.LogError("Core is missing in the EnemyState!");
            return;
        }
    }


    public override void LogicUpdate()
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
}
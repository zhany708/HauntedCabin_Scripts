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
        }
    }
}
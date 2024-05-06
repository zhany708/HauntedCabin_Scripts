public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        enemy.Parameter.Target = null;      //敌人死亡后将Target坐标清零，防止出现bug
    }

    public override void LogicUpdate() { }      //重写逻辑函数，使敌人不会在死亡状态中进入受击状态
}
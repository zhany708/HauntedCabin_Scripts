using UnityEngine;


public class FireBatAttackState_DefenseWar : EnemyAttackState
{
    FireBat_DefenseWar m_FireBat;
    Transform m_Target;

    public FireBatAttackState(FireBat_DefenseWar fireBat, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(fireBat, stateMachine, enemyData, animBoolName)
    {
        m_FireBat = fireBat;
    }



    public override void Enter()
    {
        //Debug.Log("FireBatAttackState");
        //检查是否有玩家的坐标
        if (enemy.Parameter.Target != null)
        {
            m_Target = enemy.Parameter.Target;           //储存玩家坐标信息，防止发射火球时丢失坐标
        }
        
        //检查是否有祷告石的坐标
        else if (enemy.Parameter.AltarTarget != null)
        {
            m_Target = enemy.Parameter.AltarTarget;      //储存祷告石坐标信息，防止发射火球时丢失坐标
        }

        base.Enter();
    }

    public override void LogicUpdate()
    {
        if (core.AnimatorInfo.IsName("Attack") && core.AnimatorInfo.normalizedTime >= 0.95f)     //播放完攻击动画则发射火球且切换成追击状态
        {
            if (m_Target != null)
            {
                m_FireBat.FireBallLaunch(m_Target);
            }
            
            stateMachine.ChangeState(enemy.ChaseState);     //攻击完后进入追击状态
        }
    }
}
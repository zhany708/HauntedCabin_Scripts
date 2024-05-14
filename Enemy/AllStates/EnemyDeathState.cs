using UnityEngine;




public class EnemyDeathState : EnemyState
{
    AnimatorStateInfo m_AnimatorStateInfo;



    public EnemyDeathState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        enemy.Parameter.Target = null;      //���˽�������״̬��Target�������㣬��ֹ����bug

        m_AnimatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //��ȡ��ǰ����

        if (!m_AnimatorStateInfo.IsName("Death"))       //��鵱ǰ�Ƿ��ڲ����������������û���򲥷�
        {          
            core.Animator.SetBool("Death", true);
        }
    }

    public override void LogicUpdate() { }    //����Ҫִ�д˺����ڸ����е��߼�
}
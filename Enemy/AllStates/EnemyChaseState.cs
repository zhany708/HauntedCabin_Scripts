using UnityEngine;



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
            //ʹ���˳������
            enemy.EnemyFlip.FlipX( enemy.Movement.GetFlipNum(enemy.Parameter.Target.position, enemy.transform.position) );

            //�����������ҵľ���
            m_DistanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.Parameter.Target.position);       
        }

        base.LogicUpdate();

        if (enemy.Parameter.Target == null) //|| enemy.CheckOutside())  ����ӣ���Ұ�����׷����Χʱ�л�������״̬
        {
            stateMachine.ChangeState(enemy.IdleState);      //��ʧĿ����л�������״̬
        }

        //��⹥����Χ����һ������ΪԲ��λ�ã��ڶ���Ϊ�뾶��������ΪĿ��ͼ��.��Ҵ��ڹ�����Χ�ҹ��������������빥��״̬
        else if (Physics2D.OverlapCircle(enemy.Parameter.AttackPoint.position, enemyData.AttackArea, enemyData.TargetLayer) && enemy.CanAttack)
        {
            stateMachine.ChangeState(enemy.AttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //���������������Ҿ��������С����ʱ����׷�����
        if (enemy.Parameter.Target && m_DistanceToPlayer > enemyData.StoppingDistance)     
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.Parameter.Target.position, enemyData.ChaseSpeed * Time.deltaTime);
        }
    }
}
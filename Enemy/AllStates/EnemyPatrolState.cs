using UnityEngine;


public class EnemyPatrolState : EnemyState
{
    //��ȡ�Ҿ�ͼ��
    LayerMask m_FurnitureLayerMask = LayerMask.GetMask("Furniture");


    Vector2 m_RandomPosition;       //������ɵ�����
    float m_PatrolTimer;    //����Ѳ�ߵļ�ʱ��


    //����Physics2D����ظ�����ʱ��Ҫ��X��Y��ֵ��������Y������0.5��ƫ���Ϊ�����λ�ڽŵף�
    const float m_PhysicsCheckingXPos = 2f;
    const float m_PhysicsCheckingYPos = 4f;



    public EnemyPatrolState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        //�����������
        m_RandomPosition = enemy.PatrolRandomPos.GenerateSingleRandomPos();

        //�������ȷ������û����ײ���Ҿ�
        EnsurePositionIsValid();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        m_PatrolTimer += Time.deltaTime;

        enemy.EnemyFlip.FlipX(enemyMovement.GetFlipNum(m_RandomPosition, enemy.transform.position) );  //����Ѳ�ߵ�ķ���

        if (enemy.Parameter.Target != null)     // && !enemy.CheckOutside())
        {
            stateMachine.ChangeState(enemy.ChaseState);     //Ѳ��ʱ�����⵽������л�Ϊ׷��״̬
        }

        //������Ŀ��Ѳ�ߵ��㹻��ʱ���������5��������û�е���Ѳ�ߵ㣨��ס�ˣ�����ת��������״̬
        else if (Vector2.Distance(enemy.transform.position, m_RandomPosition) <= 0.1f || m_PatrolTimer >= 5f)
        {
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (m_RandomPosition != null)
        {
            //�ƶ���Ŀ���
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, m_RandomPosition, enemyData.MoveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        m_PatrolTimer = 0f;     //�˳�ʱ����ʱ������
    }







    //ȷ�����괦û����ײ���Ҿ�
    private void EnsurePositionIsValid()
    {
        //���ڷ�ֹ����ѭ���ı��������ѭ��100��
        int attemptCount = 0;

        while (!IsPositionEmpty(m_RandomPosition) && attemptCount < 100)
        {
            m_RandomPosition = enemy.PatrolRandomPos.GenerateSingleRandomPos();
            attemptCount++;
        }

        //�������ѭ����������Ȼû�����ɳ����ʵ�����ʱ���򱨴�
        if (attemptCount >= 100)
        {
            Debug.LogError("Failed to find a valid patrol position after 100 attempts.");
        }
    }


    //�������������Ҫ���ɵ������Ƿ��мҾ�
    private bool IsPositionEmpty(Vector2 position)
    {
        //��һ������Ϊ���ĵ㣬�ڶ�������Ϊ�����δ�С���������ĸ�����һ�룩������������Ϊ�Ƕȣ����ĸ�����Ϊ����Ŀ��㼶
        Collider2D overlapCheck = Physics2D.OverlapBox(position, new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos), 0f, m_FurnitureLayerMask);
        return overlapCheck == null;
    }
}
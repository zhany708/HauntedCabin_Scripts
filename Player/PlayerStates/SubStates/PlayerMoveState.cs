using UnityEngine;


public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        //Ŀǰ��Ҳ�������״̬�����Ǻ������¹�����Ҫ���
    }


    /*
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (input.x == 0f && input.y == 0f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        
        if (!isAttack)   //��ֹ��ҹ���ʱ�����ƶ�
        {
            Movement.SetVelocity(playerData.MovementVelocity, input);       //�ڱ���������ʺţ���ʾֻ�б�����Ϊ��ʱ�Ż���ñ����ĺ�������������Unity�ڲ��ı����ϣ�
        }

        else
        {
            Movement.SetVelocityZero();     //���ƶ�״̬�У���ҹ���ʱ��ֹ�ƶ�
        }
        
    }
    */
}

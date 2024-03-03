using UnityEngine;


public class PlayerHitState : PlayerGroundedState
{
    AnimatorStateInfo m_AnimatorStateInfo;

    public PlayerHitState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isHit = true;
    }

    public override void Exit()
    {
        base.Exit();

        Combat.SetIsHit(false);
        isHit = false;
    }

    public override void LogicUpdate()
    {
        input = player.InputHandler.RawMovementInput;   //ͨ��Player�ű���������״̬���ƶ�״̬��Ҫ��������ֵ



        m_AnimatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //��ȡ��ǰ����

        if (m_AnimatorStateInfo.IsName("Hit") && m_AnimatorStateInfo.normalizedTime >= 0.95f)
        {
            stateMachine.ChangeState(player.IdleState);     //�ܻ������������л�������״̬
        }
    }

    public override void PhysicsUpdate()
    {
        //��ֹ����ܻ�ʱ�����ƶ�
    }
}

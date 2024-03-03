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
        input = player.InputHandler.RawMovementInput;   //通过Player脚本调用闲置状态和移动状态需要的向量数值



        m_AnimatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //获取当前动画

        if (m_AnimatorStateInfo.IsName("Hit") && m_AnimatorStateInfo.normalizedTime >= 0.95f)
        {
            stateMachine.ChangeState(player.IdleState);     //受击动画结束后切换成闲置状态
        }
    }

    public override void PhysicsUpdate()
    {
        //禁止玩家受击时自由移动
    }
}

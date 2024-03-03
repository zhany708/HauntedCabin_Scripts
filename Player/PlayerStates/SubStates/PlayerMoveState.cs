using UnityEngine;


public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();

        //player.FootAnimator.SetBool("Move", true);      //设置玩家的脚上的动画器参数
    }

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

        /*
        if (!isAttack)   //禁止玩家攻击时自由移动
        {
            Movement.SetVelocity(playerData.MovementVelocity, input);       //在变量后面加问号，表示只有变量不为空时才会调用变量的函数（不能用在Unity内部的变量上）
        }

        else
        {
            Movement.SetVelocityZero();     //在移动状态中，玩家攻击时禁止移动
        }
        */

        Movement.SetVelocity(playerData.MovementVelocity, input);       //在变量后面加问号，表示只有变量不为空时才会调用变量的函数（不能用在Unity内部的变量上）
    }

    public override void Exit()
    {
        base.Exit();

        //player.FootAnimator.SetBool("Move", false);
    }
}

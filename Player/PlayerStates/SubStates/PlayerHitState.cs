public class PlayerHitState : PlayerGroundedState
{
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

        combat.SetIsHit(false);
        isHit = false;
    }

    public override void LogicUpdate()
    {
        //检查玩家是否死亡
        if (playerStats.GetCurrentHealth() <= 0)
        {
            stateMachine.ChangeState(player.DeathState);
        }

        
        input = player.InputHandler.RawMovementInput;   //通过Player脚本调用闲置状态和移动状态需要的向量数值



        animatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //获取当前动画

        if (animatorStateInfo.IsName("Hit") && animatorStateInfo.normalizedTime >= 0.95f)
        {
            stateMachine.ChangeState(player.IdleState);     //受击动画结束后切换成闲置状态
        }
    }

    public override void PhysicsUpdate()
    {
        //重写此函数，从而禁止玩家受击时自由移动
    }
}
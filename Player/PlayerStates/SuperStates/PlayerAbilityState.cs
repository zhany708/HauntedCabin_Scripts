public class PlayerAbilityState : PlayerState
{
    protected bool isAbilityDone;       //用于表示玩家是否在使用特殊能力（目前只有普通攻击）


    public PlayerAbilityState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAbilityDone)
        {
            stateMachine.ChangeState(player.IdleState);     //攻击结束后进入闲置状态
        }
    }
}

using UnityEngine;



public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }



    public override async void Enter()
    {
        base.Enter();

        //提前加载好游戏失败界面
        await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.GameLostPanel);

        EnvironmentManager.Instance.SetIsGameLost(true);        //设置布尔，表示游戏失败，同时防止《地狱的呼唤》中仪式结束后执行相关的逻辑

        player.FootAnimator.gameObject.SetActive(false);        //取消激活玩家的脚

        //取消激活所有武器
        player.PrimaryWeapon.gameObject.SetActive(false);
        player.SecondaryWeapon.gameObject.SetActive(false);
    }

    public override void LogicUpdate()
    {
        animatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //获取当前动画

        if (animatorStateInfo.IsName("Death") && animatorStateInfo.normalizedTime >= 0.95f)
        {
            //Debug.Log("Game Over!");

            //死亡动画播完后进入“闲置”状态（实际上直接退出玩家状态机）
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate() { }

    public override async void Exit()
    {
        base.Exit();

        player.gameObject.SetActive(false);    //取消激活玩家，从而退出玩家状态机

        //打开游戏失败界面不能放在LogicUpdate函数里，防止重复打开多个界面
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameLostPanel);
    }
}
using UnityEngine;



public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
      
        player.FootAnimator.gameObject.SetActive(false);    //取消激活玩家的脚

        //取消激活所有武器
        player.PrimaryWeapon.gameObject.SetActive(false);
        player.SecondaryWeapon.gameObject.SetActive(false);
    }

    public override async void Exit()
    {
        base.Exit();

        player.gameObject.SetActive(false);    //取消激活玩家，从而退出玩家状态机

        //游戏结束界面不能放在LogicUpdate函数里，防止重复打开多个界面
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameOverPanel);
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
}
using System.Diagnostics;
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



    public override void LogicUpdate()
    {
        animatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //获取当前动画

        if (animatorStateInfo.IsName("Death") && animatorStateInfo.normalizedTime >= 0.95f)
        {
            player.gameObject.SetActive(false);     //取消激活玩家，防止出现鞭尸现象

            //死亡动画播完后打开游戏结束界面
            UnityEngine.Debug.Log("Game Over!");
        }
    }

    public override void PhysicsUpdate() { }
}
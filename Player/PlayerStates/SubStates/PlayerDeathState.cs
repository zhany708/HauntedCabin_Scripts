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
      
        player.FootAnimator.gameObject.SetActive(false);    //ȡ��������ҵĽ�

        //ȡ��������������
        player.PrimaryWeapon.gameObject.SetActive(false);
        player.SecondaryWeapon.gameObject.SetActive(false);
    }



    public override void LogicUpdate()
    {
        animatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //��ȡ��ǰ����

        if (animatorStateInfo.IsName("Death") && animatorStateInfo.normalizedTime >= 0.95f)
        {
            player.gameObject.SetActive(false);     //ȡ��������ң���ֹ���ֱ�ʬ����

            //����������������Ϸ��������
            UnityEngine.Debug.Log("Game Over!");
        }
    }

    public override void PhysicsUpdate() { }
}
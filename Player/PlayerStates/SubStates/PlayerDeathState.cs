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

    public override async void Exit()
    {
        base.Exit();

        player.gameObject.SetActive(false);    //ȡ��������ң��Ӷ��˳����״̬��

        //��Ϸ�������治�ܷ���LogicUpdate�������ֹ�ظ��򿪶������
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameOverPanel);
    }




    public override void LogicUpdate()
    {
        animatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //��ȡ��ǰ����

        if (animatorStateInfo.IsName("Death") && animatorStateInfo.normalizedTime >= 0.95f)
        {
            //Debug.Log("Game Over!");

            //���������������롰���á�״̬��ʵ����ֱ���˳����״̬����
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate() { }
}
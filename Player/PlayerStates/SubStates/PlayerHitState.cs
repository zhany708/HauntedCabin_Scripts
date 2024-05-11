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
        //�������Ƿ�����
        if (playerStats.GetCurrentHealth() <= 0)
        {
            stateMachine.ChangeState(player.DeathState);
        }

        
        input = PlayerInputHandler.Instance.RawMovementInput;   //ͨ��Player�ű���������״̬���ƶ�״̬��Ҫ��������ֵ



        animatorStateInfo = core.Animator.GetCurrentAnimatorStateInfo(0);       //��ȡ��ǰ����

        if (animatorStateInfo.IsName("Hit") && animatorStateInfo.normalizedTime >= 0.95f)
        {
            stateMachine.ChangeState(player.IdleState);     //�ܻ������������л�������״̬
        }
    }

    public override void PhysicsUpdate()
    {
        //��д�˺������Ӷ���ֹ����ܻ�ʱ�����ƶ�
    }
}
using UnityEngine;



public class PlayerState
{
    protected Core core;
    
    protected Movement movement
    {
        get
        {
            if (m_Movement) { return m_Movement; }      //�������Ƿ�Ϊ��
            m_Movement = core.GetCoreComponent<Movement>();
            return m_Movement;
        }
    }
    private Movement m_Movement; 
    
    
    protected Combat combat
    {
        get
        {
            if (m_Combat) { return m_Combat; }
            m_Combat = core.GetCoreComponent<Combat>();
            return m_Combat;
        }
    }
    private Combat m_Combat;


    protected PlayerStats playerStats
    {
        get
        {
            if (m_PlayerStats) { return m_PlayerStats; }
            m_PlayerStats = core.GetCoreComponent<PlayerStats>();
            return m_PlayerStats;
        }
    }
    private PlayerStats m_PlayerStats;


    protected AnimatorStateInfo animatorStateInfo;

    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected SO_PlayerData playerData;


    protected Vector2 input;        //����״̬���ƶ�״̬��Ҫ��������ֵ

    protected bool isAnimationFinished = false;     //���ڼ�鶯���Ƿ񲥷����
    protected bool isAttack = false;
    protected bool isHit = false;


    string m_AnimationBoolName;     //���߶�����Ӧ�ò����ĸ�����


    public PlayerState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        m_AnimationBoolName = animBoolName;
        core = player.Core;
    }


    public virtual void Enter()
    {
        //Debug.Log(m_AnimationBoolName);

        player.Core.Animator.SetBool(m_AnimationBoolName, true);     //����״̬�Ķ���
    }

    public virtual void Exit()
    {
        player.Core.Animator.SetBool(m_AnimationBoolName, false);        //���õ�ǰ״̬����Ϊfalse�Խ����¸�״̬
    }

    public virtual void LogicUpdate() 
    {
        input = player.InputHandler.RawMovementInput;   //ͨ��Player�ű���������״̬���ƶ�״̬��Ҫ��������ֵ

        SetMoveAnimation();    //�ж��Ƿ񲥷ŽŲ��ƶ�����

        if (combat.IsHit && !isHit && !isAttack)    //����Ƿ�����ܻ�״̬
        {
            stateMachine.ChangeState(player.HitState);
        }
    }


    public virtual void PhysicsUpdate() 
    {
        //ֻ�е�û�д���ť�Ľ����ʱ������������ƶ�
        if (!PanelWithButton.IsPanelWithButtonOpened)
        {
            //���ƶ��߼����ڸ�״̬�У����������������ƶ�״̬Ҳ�����ƶ���ĳЩ״̬��Ҫ���Ǵ˺��������ܻ�״̬��
            movement.SetVelocity(playerData.MovementVelocity, input);
        }

        //������ͣ��ҵ��ƶ�
        else
        {
            movement.SetVelocityZero();
        }
    }
    


    public virtual void AnimationFinishTrigger() => isAnimationFinished = true; 


    protected void SetMoveAnimation()      //�������Ƿ���WASD�����Ӷ��ж��Ƿ񲥷ŽŲ��ƶ�����
    {
        if (input.x == 0f && input.y == 0f)
        {
            player.FootAnimator.SetBool("Move", false);
        }
        else
        {
            player.FootAnimator.SetBool("Move", true);
        }
    }
}

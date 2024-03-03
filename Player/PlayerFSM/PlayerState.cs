using UnityEngine;

public class PlayerState
{
    protected Core core;
    
    protected Movement Movement
    {
        get
        {
            if (m_Movement) { return m_Movement; }      //�������Ƿ�Ϊ��
            m_Movement = core.GetCoreComponent<Movement>();
            return m_Movement;
        }
    }
    private Movement m_Movement; 
    
    
    protected Combat Combat
    {
        get
        {
            if (m_Combat) { return m_Combat; }
            m_Combat = core.GetCoreComponent<Combat>();
            return m_Combat;
        }
    }
    private Combat m_Combat;
    


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

        if (Combat.IsHit && !isHit && !isAttack)    //����Ƿ�����ܻ�״̬
        {
            stateMachine.ChangeState(player.HitState);
        }
    }


    public virtual void PhysicsUpdate() 
    {
        Movement.SetVelocity(playerData.MovementVelocity, input);       //���ƶ��߼����ڸ�״̬�У����������������ƶ�״̬Ҳ�����ƶ���ĳЩ״̬��Ҫ���Ǵ˺��������ܻ�״̬��
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

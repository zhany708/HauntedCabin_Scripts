using UnityEngine;

public class PlayerState
{
    protected Core core;
    
    protected Movement Movement
    {
        get
        {
            if (m_Movement) { return m_Movement; }      //检查组件是否为空
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

    //protected float startTime;    //用于检查每个状态的持续时间

    protected Vector2 input;        //闲置状态和移动状态需要的向量数值

    protected bool isAnimationFinished = false;     //用于检查动画是否播放完毕
    protected bool canAttack;
    protected bool isAttack = false;
    protected bool isHit = false;


    string m_AnimationBoolName;     //告诉动画器应该播放哪个动画


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
        player.Core.Animator.SetBool(m_AnimationBoolName, true);     //播放状态的动画

        //Debug.Log(m_AnimationBoolName);
    }

    public virtual void Exit()
    {
        player.Core.Animator.SetBool(m_AnimationBoolName, false);        //设置当前状态布尔为false以进入下个状态
    }

    public virtual void LogicUpdate() 
    { 
        CheckIsMoving();    //判断是否播放脚步移动动画

        if (Combat.IsHit && !isHit && !isAttack)    //检查是否进入受击状态
        {
            stateMachine.ChangeState(player.HitState);
        }
    }


    public virtual void PhysicsUpdate() { }
    
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true; 


    protected void CheckIsMoving()      //检查玩家是否按下WASD键，从而判断是否播放脚步移动动画
    {
        input = player.InputHandler.RawMovementInput;   //通过Player脚本调用闲置状态和移动状态需要的向量数值

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

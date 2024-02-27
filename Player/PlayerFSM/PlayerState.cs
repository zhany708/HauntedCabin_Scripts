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
    protected bool isAnimationFinished = false;     //用于检查动画是否播放完毕
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
        if (Combat.IsHit && !isHit && !isAttack)
        {
            stateMachine.ChangeState(player.HitState);
        }
    }


    public virtual void PhysicsUpdate() { }
    
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true; 
}

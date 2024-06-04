using UnityEngine;



public class PlayerState
{
    protected Core core;
    
    protected Movement movement
    {
        get
        {
            if (m_Movement) { return m_Movement; }      //检查组件是否为空
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


    protected Vector2 input;        //闲置状态和移动状态需要的向量数值

    protected bool isAnimationFinished = false;     //用于检查动画是否播放完毕
    protected bool isAttack = false;        //表示是否正在攻击
    protected bool isHit = false;           //表示是否正在受击


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
        //Debug.Log(m_AnimationBoolName);

        player.Core.Animator.SetBool(m_AnimationBoolName, true);     //播放状态的动画
    }

    public virtual void Exit()
    {
        player.Core.Animator.SetBool(m_AnimationBoolName, false);        //设置当前状态布尔为false以进入下个状态
    }


    public virtual void LogicUpdate() 
    {
        input = PlayerInputHandler.Instance.RawMovementInput;   //通过Player脚本调用闲置状态和移动状态需要的向量数值

        SetMoveAnimation();    //判断是否播放脚步移动动画


        //检查玩家是否死亡（不能放在受击状态里判断，因为玩家攻击时不会进入受击状态）
        if (playerStats.GetCurrentHealth() <= 0)
        {
            stateMachine.ChangeState(player.DeathState);
        }     

        else if (combat.IsHit && !isHit && !isAttack)    //检查是否进入受击状态
        {
            stateMachine.ChangeState(player.HitState);
        }
    }

    public virtual void PhysicsUpdate() 
    {
        //只有当玩家可以移动，才允许玩家移动
        if (BasePanel.IsPlayerMoveable)
        {
            //将移动逻辑放在跟状态中，这样玩家无需进入移动状态也可以移动（某些状态需要覆盖此函数，如受击状态）
            movement.SetVelocity(playerData.MovementVelocity * PlayerStatusBar.GetSpeedAddition(), input);
        }

        //否则暂停玩家的移动
        else
        {
            movement.SetVelocityZero();
        }
    }
    


    public virtual void AnimationFinishTrigger() => isAnimationFinished = true; 





    protected void SetMoveAnimation()      //检查玩家是否按下WASD键，从而判断是否播放脚步移动动画
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
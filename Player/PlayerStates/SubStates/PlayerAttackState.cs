public class PlayerAttackState : PlayerAbilityState
{

    Weapon m_Weapon;







    #region 状态机内部函数
    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName, Weapon weapon) : base(player, stateMachine, playerData, animBoolName)
    {
        m_Weapon = weapon;

        m_Weapon.OnWeaponExit += ExitHandler;
    }

    public override void Enter()
    {
        //Debug.Log("You entered Attack state!");

        base.Enter();

        isAttack = true;

        m_Weapon.EnterWeapon();
    }

    /*
    public override void Exit()
    {
        Debug.Log("You exited Attack state!");

        base.Exit();
    }
    */
    #endregion


    #region 绑定回调事件的函数
    private void ExitHandler()
    {
        AnimationFinishTrigger();

        isAttack = false;
        isAbilityDone = true;       //设置isAbilityDone为true以离开攻击状态
    }
    #endregion


    /*
    public void ChangeWeapon(Weapon weapon)
    {
        if (m_Weapon != null)
        {
            m_Weapon = weapon;
        }
    }
    */
}
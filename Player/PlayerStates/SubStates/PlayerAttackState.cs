public class PlayerAttackState : PlayerAbilityState
{

    Weapon m_Weapon;




    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName, Weapon weapon) : base(player, stateMachine, playerData, animBoolName)
    {
        m_Weapon = weapon;

        m_Weapon.OnExit += ExitHandler;
    }

    public override void Enter()
    {
        base.Enter();

        isAttack = true;

        m_Weapon.EnterWeapon();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Movement.SetVelocity(playerData.MovementVelocity, player.InputHandler.RawMovementInput);
    }




    private void ExitHandler()
    {
        AnimationFinishTrigger();

        isAttack = false;
        isAbilityDone = true;
    }

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
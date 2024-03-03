using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{

    Weapon m_Weapon;

    float m_AttackCooldownTime = 0;






    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName, Weapon weapon) : base(player, stateMachine, playerData, animBoolName)
    {
        m_Weapon = weapon;

        if (m_Weapon is GunWeapon)
        {
            GunWeapon gunWeapon = (GunWeapon)m_Weapon;

            m_AttackCooldownTime = gunWeapon.GunData.AttackDetail.AttackCooldownTime;
        }

        m_Weapon.OnExit += ExitHandler;
    }

    public override void Enter()
    {
        Debug.Log("You entered Attack state!");

        //player.InputHandler.ResetAttackInputs();

        base.Enter();


        isAttack = true;

        m_Weapon.EnterWeapon();
    }

    public override void Exit()
    {
        Debug.Log("You exited Attack state!");

        base.Exit();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Movement.SetVelocity(playerData.MovementVelocity, input);
    }




    private void ExitHandler()
    {
        AnimationFinishTrigger();

        isAttack = false;
        isAbilityDone = true;       //设置isAbilityDone为真以进入闲置状态
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
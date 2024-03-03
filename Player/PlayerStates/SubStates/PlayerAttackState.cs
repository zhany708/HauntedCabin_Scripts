using System.Collections;
using UnityEngine;

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
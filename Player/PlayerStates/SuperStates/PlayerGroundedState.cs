using UnityEngine;


public class PlayerGroundedState : PlayerState
{

    protected Vector2 input;


    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        input = player.InputHandler.RawMovementInput;   //通过Player脚本调用闲置状态和移动状态需要的向量数值


        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary])        //按下鼠标左键时，过渡到主武器攻击状态
        {
            player.PrimaryWeapon.transform.parent.gameObject.SetActive(true);       //启用主武器库
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(false);    //禁用副武器库

            stateMachine.ChangeState(player.PrimaryAttackState);
        }


        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary])     //按下鼠标右键时，过渡到副武器攻击状态
        {
            player.PrimaryWeapon.transform.parent.gameObject.SetActive(false);
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(true);

            stateMachine.ChangeState(player.SecondaryAttackState);
        }
    }      
}

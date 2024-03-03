using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


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
            //先检查当前武器是否为远程武器（使用is时继承的类也算），再检查是否处于未激活状态。如果都为真则在激活前就赋值鼠标坐标
            if (player.PrimaryWeapon is GunWeapon && !player.PrimaryWeapon.transform.parent.gameObject.activeSelf)
            {
                //Debug.Log("Mouse position is updated!");
                player.PrimaryWeapon.SetMousePosition( player.InputHandler.ProjectedMousePos - new Vector2(player.PrimaryWeapon.transform.position.x, player.PrimaryWeapon.transform.position.y) );    //计算需要朝向鼠标的方向);
            }


            player.PrimaryWeapon.transform.parent.gameObject.SetActive(true);       //启用主武器库
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(false);    //禁用副武器库

            stateMachine.ChangeState(player.PrimaryAttackState);
        }


        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary])     //按下鼠标右键时，过渡到副武器攻击状态
        {
            //先检查当前武器是否为远程武器，再检查是否处于未激活状态。如果都为真则在激活前就赋值鼠标坐标
            if (player.SecondaryWeapon is GunWeapon && !player.SecondaryWeapon.transform.parent.gameObject.activeSelf)
            {
                //Debug.Log("Mouse position is updated!");
                player.SecondaryWeapon.SetMousePosition(player.InputHandler.ProjectedMousePos - new Vector2(player.SecondaryWeapon.transform.position.x, player.SecondaryWeapon.transform.position.y));    //计算需要朝向鼠标的方向);
            }


            player.PrimaryWeapon.transform.parent.gameObject.SetActive(false);
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(true);

            stateMachine.ChangeState(player.SecondaryAttackState);
        }
    }      
}

using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();


        //检查是否进入攻击状态
        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary] && stateMachine.currentState != player.PrimaryAttackState)        //按下鼠标左键时，进入主武器攻击状态
        {
            //先检查当前武器是否为远程武器（使用is时继承的类也会返回真），再检查是否处于未激活状态。如果都为真则在激活前就赋值鼠标坐标
            if (player.PrimaryWeapon is GunWeapon && !player.PrimaryWeapon.transform.parent.gameObject.activeSelf)
            {
                //Debug.Log("Mouse position is updated!");
                player.PrimaryWeapon.SetMousePosition( player.InputHandler.ProjectedMousePos - new Vector2(player.PrimaryWeapon.transform.position.x, player.PrimaryWeapon.transform.position.y) );    //计算需要朝向鼠标的方向);
            }


            player.PrimaryWeapon.transform.parent.gameObject.SetActive(true);       //启用主武器库
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(false);    //禁用副武器库

            stateMachine.ChangeState(player.PrimaryAttackState);
        }


        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary] && stateMachine.currentState != player.SecondaryAttackState)     //按下鼠标右键时，进入副武器攻击状态
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

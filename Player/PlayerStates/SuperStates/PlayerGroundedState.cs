public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //游戏没有暂停，且没有带按钮的界面打开时才能进行攻击
        if (!PauseMenuPanel.IsGamePaused && !PanelWithButton.IsPanelWithButtonOpened)
        {
            //检查是否进入攻击状态
            if (PlayerInputHandler.Instance.AttackInputs[(int)CombatInputs.primary] && stateMachine.currentState != player.PrimaryAttackState)        //按下鼠标左键时，进入主武器攻击状态
            {
                player.MakeSpriteVisible(player.PrimaryWeapon.transform.gameObject, true);      //显示当前装备的主武器
                player.MakeSpriteVisible(player.SecondaryWeapon.transform.gameObject, false);   //隐藏当前装备的主武器

                stateMachine.ChangeState(player.PrimaryAttackState);
            }


            else if (PlayerInputHandler.Instance.AttackInputs[(int)CombatInputs.secondary] && stateMachine.currentState != player.SecondaryAttackState)     //按下鼠标右键时，进入副武器攻击状态
            {
                player.MakeSpriteVisible(player.PrimaryWeapon.transform.gameObject, false);
                player.MakeSpriteVisible(player.SecondaryWeapon.transform.gameObject, true);

                stateMachine.ChangeState(player.SecondaryAttackState);
            }
        }       
    }      
}

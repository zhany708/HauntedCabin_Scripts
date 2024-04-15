public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, SO_PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();


        //����Ƿ���빥��״̬
        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary] && stateMachine.currentState != player.PrimaryAttackState)        //����������ʱ����������������״̬
        {
            player.MakeSpriteVisible(player.PrimaryWeapon.transform.gameObject, true);      //��ʾ��ǰװ����������
            player.MakeSpriteVisible(player.SecondaryWeapon.transform.gameObject, false);   //���ص�ǰװ����������

            stateMachine.ChangeState(player.PrimaryAttackState);
        }


        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary] && stateMachine.currentState != player.SecondaryAttackState)     //��������Ҽ�ʱ�����븱��������״̬
        {
            player.MakeSpriteVisible(player.PrimaryWeapon.transform.gameObject, false);
            player.MakeSpriteVisible(player.SecondaryWeapon.transform.gameObject, true);

            stateMachine.ChangeState(player.SecondaryAttackState);
        }
    }      
}

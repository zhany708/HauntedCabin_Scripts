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


        //����Ƿ���빥��״̬
        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary] && stateMachine.currentState != player.PrimaryAttackState)        //����������ʱ����������������״̬
        {
            //�ȼ�鵱ǰ�����Ƿ�ΪԶ��������ʹ��isʱ�̳е���Ҳ�᷵���棩���ټ���Ƿ���δ����״̬�������Ϊ�����ڼ���ǰ�͸�ֵ�������
            if (player.PrimaryWeapon is GunWeapon && !player.PrimaryWeapon.transform.parent.gameObject.activeSelf)
            {
                //Debug.Log("Mouse position is updated!");
                player.PrimaryWeapon.SetMousePosition( player.InputHandler.ProjectedMousePos - new Vector2(player.PrimaryWeapon.transform.position.x, player.PrimaryWeapon.transform.position.y) );    //������Ҫ�������ķ���);
            }


            player.PrimaryWeapon.transform.parent.gameObject.SetActive(true);       //������������
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(false);    //���ø�������

            stateMachine.ChangeState(player.PrimaryAttackState);
        }


        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary] && stateMachine.currentState != player.SecondaryAttackState)     //��������Ҽ�ʱ�����븱��������״̬
        {
            //�ȼ�鵱ǰ�����Ƿ�ΪԶ���������ټ���Ƿ���δ����״̬�������Ϊ�����ڼ���ǰ�͸�ֵ�������
            if (player.SecondaryWeapon is GunWeapon && !player.SecondaryWeapon.transform.parent.gameObject.activeSelf)
            {
                //Debug.Log("Mouse position is updated!");
                player.SecondaryWeapon.SetMousePosition(player.InputHandler.ProjectedMousePos - new Vector2(player.SecondaryWeapon.transform.position.x, player.SecondaryWeapon.transform.position.y));    //������Ҫ�������ķ���);
            }


            player.PrimaryWeapon.transform.parent.gameObject.SetActive(false);
            player.SecondaryWeapon.transform.parent.gameObject.SetActive(true);

            stateMachine.ChangeState(player.SecondaryAttackState);
        }
    }      
}

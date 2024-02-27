using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }       //��ֹ�ֱ���΢�ƶ�ʱ����ٶȻ�����ǿ����0-1֮���С����Ϊ1��
    public Vector2 ProjectedMousePos { get; private set; }      //���ڳ�������������꣨��ʹ��꾲ֹ��
    public Vector2 MouseScrollInput { get; private set; }       //�����ֵ���Ϣ


    public bool[] AttackInputs { get; private set; }    //���ڼ���������͸�����
    public bool IsSpacePressed {  get; private set; }   //���ڱ�ʾ�Ƿ��¿ո�

    Vector2 m_MousePos;






    private void Start()
    {
        int count = Enum.GetValues(typeof(CombatInputs)).Length;        //���ض���Ҳ�������ֹ�������

        AttackInputs = new bool[count];
    }

    private void Update()
    {
        if (m_MousePos != null)
        {
            ProjectedMousePos = Camera.main.ScreenToWorldPoint(m_MousePos);        //�����������������ĳ��������
        }
    }




    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>().normalized;   //(0,1) (0,-1) (1,0) (-1,0)����������ʾ����
    }



    public void OnPrimaryAttackInput(InputAction.CallbackContext context) 
    {
        if (context.started)    //����������ʱ
        {
            AttackInputs[(int)CombatInputs.primary] = true;
        }

        if (context.canceled)   //�ɿ�������ʱ
        {
            AttackInputs[(int)CombatInputs.primary] = false;
        }
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)    //��������Ҽ�ʱ
        {
            AttackInputs[(int)CombatInputs.secondary] = true;
        }

        if (context.canceled)
        {
            AttackInputs[(int)CombatInputs.secondary] = false;
        }
    }



    public void OnAim(InputAction.CallbackContext context)      //���ڶ�ȡ�������
    {
        m_MousePos = context.ReadValue<Vector2>();
    }


    public void OnMouseScorll(InputAction.CallbackContext context)      //���ڶ�ȡ������
    {
        MouseScrollInput = context.ReadValue<Vector2>().normalized;

        //Debug.Log(MouseScrollInput);
    }


    public void OnSpacebar(InputAction.CallbackContext context)    //���ڶ�ȡ���̿ո�
    {
        IsSpacePressed = context.performed;     //���¿ո�ʱΪ�棬�ɿ���Ϊ��
    }
}




public enum CombatInputs
{ 
    primary,
    secondary
}

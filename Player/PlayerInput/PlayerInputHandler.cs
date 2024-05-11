using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }


    public Vector2 RawMovementInput { get; private set; }       //��ֹ�ֱ���΢�ƶ�ʱ����ٶȻ�����ǿ����0-1֮���С����Ϊ1��
    public Vector2 ProjectedMousePos { get; private set; }      //���ڳ�������������꣨��ʹ��꾲ֹ��
    public Vector2 MouseScrollInput { get; private set; }       //�����ֵ���Ϣ


    public bool[] AttackInputs { get; private set; }    //���ڼ����갴��������ʹ��������������
    public bool IsSpacePressed {  get; private set; }   //���ڱ�ʾ�Ƿ��¿ո�
    public bool IsEscPressed { get; private set; }      //���ڱ�ʾ�Ƿ���Esc��

    Vector2 m_MousePos;

    bool m_CanDetectAttack = true;   //�����Ƿ�����갴������Ҫ����UIʱȡ����⣩





    #region Unity Callback Functions
    private void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

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
    #endregion

    #region CallbackContexts
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();   //(0,1) (0,-1) (1,0) (-1,0)����������ʾ����
        RawMovementInput = input.magnitude > 0 ? input.normalized : Vector2.zero;
    }



    public void OnPrimaryAttackInput(InputAction.CallbackContext context) 
    {       
        if (m_CanDetectAttack)
        {
            AttackInputs[(int)CombatInputs.primary] = context.performed;
        }   
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        if (m_CanDetectAttack)
        {
            AttackInputs[(int)CombatInputs.secondary] = context.performed;
        }
    }



    public void OnMousePos(InputAction.CallbackContext context)      //���ڶ�ȡ�������
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

    public void OnEscKey(InputAction.CallbackContext context)    //���ڶ�ȡ����Esc
    {
        IsEscPressed = context.performed;     //����EscʱΪ�棬�ɿ���Ϊ��
    }
    #endregion

    #region Setters
    public void SetCanDetectAttack(bool canDetectAttack)
    {
        m_CanDetectAttack = canDetectAttack;
    }

    /*
    public void ResetAttackInputs()
    {
        for (int i = 0; i < AttackInputs.Length; i++)
        {
            AttackInputs[i] = false;
        }
    }
    */
    #endregion
}




public enum CombatInputs
{ 
    primary,
    secondary
}
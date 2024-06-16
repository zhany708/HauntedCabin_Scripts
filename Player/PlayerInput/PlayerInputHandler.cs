using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }


    public Vector2 RawMovementInput { get; private set; }       //防止手柄轻微移动时玩家速度缓慢（强制让0-1之间的小数设为1）
    public Vector2 ProjectedMousePos { get; private set; }      //用于持续更新鼠标坐标（即使鼠标静止）
    public Vector2 MouseScrollInput { get; private set; }       //鼠标滚轮的信息


    public bool[] AttackInputs { get; private set; }          //用于检测鼠标按键，决定使用主武器或副武器
    public bool IsSpacePressed {  get; private set; }         //用于表示是否按下空格
    public bool IsEscPressed { get; private set; }            //用于表示是否按下Esc键
    public bool IsInteractKeyPressed { get; private set; }    //用于表示是否按下互动按键（默认F）

    Vector2 m_MousePos;






    #region Unity Callback Functions
    private void Awake()
    {
        //单例模式
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
        int count = Enum.GetValues(typeof(CombatInputs)).Length;        //返回二，也就是两种攻击武器

        AttackInputs = new bool[count];
    }

    private void Update()
    {
        if (m_MousePos != null)
        {
            ProjectedMousePos = Camera.main.ScreenToWorldPoint(m_MousePos);        //将鼠标坐标从相对相机改成相对世界
        }
    }
    #endregion

    #region CallbackContexts
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();   //(0,1) (0,-1) (1,0) (-1,0)四种向量表示方向
        RawMovementInput = input.magnitude > 0 ? input.normalized : Vector2.zero;
    }



    public void OnPrimaryAttackInput(InputAction.CallbackContext context) 
    {       
        AttackInputs[(int)CombatInputs.primary] = context.performed;       
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {      
        AttackInputs[(int)CombatInputs.secondary] = context.performed;      
    }



    public void OnMousePos(InputAction.CallbackContext context)      //用于读取鼠标坐标
    {
        m_MousePos = context.ReadValue<Vector2>();
    }


    public void OnMouseScorll(InputAction.CallbackContext context)      //用于读取鼠标滚轮
    {
        MouseScrollInput = context.ReadValue<Vector2>().normalized;

        //Debug.Log(MouseScrollInput);
    }


    public void OnSpacebar(InputAction.CallbackContext context)    //用于读取键盘空格
    {
        IsSpacePressed = context.performed;     //按下空格时为真，松开后为假
    }

    public void OnEscKey(InputAction.CallbackContext context)    //用于读取键盘Esc
    {
        IsEscPressed = context.performed;     //按下Esc时为真，松开后为假
    }

    public void OnInteractKey(InputAction.CallbackContext context)    //用于读取键盘互动按键
    {
        IsInteractKeyPressed = context.performed;     //按下互动按键（默认F）时为真，松开后为假
    }
    #endregion

    #region Setters
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
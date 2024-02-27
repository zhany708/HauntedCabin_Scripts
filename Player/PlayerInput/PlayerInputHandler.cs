using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }       //防止手柄轻微移动时玩家速度缓慢（强制让0-1之间的小数设为1）
    public Vector2 ProjectedMousePos { get; private set; }      //用于持续更新鼠标坐标（即使鼠标静止）
    public Vector2 MouseScrollInput { get; private set; }       //鼠标滚轮的信息


    public bool[] AttackInputs { get; private set; }    //用于检测主武器和副武器
    public bool IsSpacePressed {  get; private set; }   //用于表示是否按下空格

    Vector2 m_MousePos;






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




    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>().normalized;   //(0,1) (0,-1) (1,0) (-1,0)四种向量表示方向
    }



    public void OnPrimaryAttackInput(InputAction.CallbackContext context) 
    {
        if (context.started)    //按下鼠标左键时
        {
            AttackInputs[(int)CombatInputs.primary] = true;
        }

        if (context.canceled)   //松开鼠标左键时
        {
            AttackInputs[(int)CombatInputs.primary] = false;
        }
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)    //按下鼠标右键时
        {
            AttackInputs[(int)CombatInputs.secondary] = true;
        }

        if (context.canceled)
        {
            AttackInputs[(int)CombatInputs.secondary] = false;
        }
    }



    public void OnAim(InputAction.CallbackContext context)      //用于读取鼠标坐标
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
}




public enum CombatInputs
{ 
    primary,
    secondary
}

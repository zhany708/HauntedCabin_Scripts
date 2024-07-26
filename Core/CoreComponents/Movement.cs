using UnityEngine;



public class Movement : CoreComponent   //用于管理移动
{
    public Rigidbody2D Rigidbody2d {  get; private set; }

    public Vector2 FacingDirection { get; private set; } = Vector2.zero;


    Vector2 m_WorkSpace = Vector2.zero;         //用于设置速度时内部的计算


    /*
    //用于检查物体是否正在移动
    Vector2 m_LastPosition;                     //上次记录的位置
    float m_Threshold = 0.01f;                  //用于计算物体是否正在移动的间隔（检查0.01秒前的坐标和当前的坐标）
    bool m_IsMoving = false;
    */



    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();

        Rigidbody2d = GetComponentInParent<Rigidbody2D>();
        if (Rigidbody2d == null)
        {
            Debug.LogError("Cannot get the Rigidbody2D component in the parent of:" + gameObject.name);
            return;
        }
    }

    private void Start()
    {
        //m_LastPosition = transform.position;        //初始化上一次记录的坐标
    }

    public override void LogicUpdate() 
    { 
        /*
        //检查物体当前是否正在移动
        Vector2 currentPos = transform.position;
        if (Vector2.Distance(m_LastPosition, currentPos) > m_Threshold )
        {
            m_IsMoving = true;

            m_LastPosition = currentPos;            //重新赋值上一次记录的坐标
        }
        else
        {
            m_IsMoving = false;
        }
        */
    }
    #endregion


    #region 设置速度相关
    public void SetVelocityZero()
    {
        m_WorkSpace = Vector2.zero;

        SetFinalVelocity();
    }

    public void ReduceVelocity(float reduceAmount)
    {
        m_WorkSpace *= (1 - reduceAmount);
        SetFinalVelocity();
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        m_WorkSpace = velocity * direction;
        
        SetFinalVelocity();
    }

    /*
    public void SetVelocity(float velocity)
    {
        m_WorkSpace *= velocity;

        SetFinalVelocity();
    }
    */

    public void SetFinalVelocity()
    {
        Rigidbody2d.velocity = m_WorkSpace;

        if (m_WorkSpace != Vector2.zero)        
        {
            FacingDirection = m_WorkSpace.normalized;   //设置角色的朝向方向
        }       
    }
    #endregion


    #region 其余函数
    public int GetFlipNum(Vector2 faceDirection, Vector2 currentDirection)      //如果不需要减去当前坐标，则第二个参数用Vector2.Zero
    {
        if (faceDirection == null)
        {
            Debug.LogError("The parameter in the function cannot be null!");
            return 0;
        }

        else
        {
            Vector2 direction = (faceDirection - currentDirection).normalized;      //只需要方向

            int facingNum = direction.x < 0 ? -1 : 1;     //如果目标坐标位于当前坐标左侧，则翻转
            return facingNum;
        }
    }
    #endregion


    #region Getters
    /*
    public bool GetIsMoving()
    {
        return m_IsMoving;
    }
    */
    #endregion
}
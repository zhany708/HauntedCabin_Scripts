using UnityEngine;



public class Movement : CoreComponent   //用于管理移动
{
    public Rigidbody2D Rigidbody2d {  get; private set; }

    public Vector2 FacingDirection { get; private set; }


    Vector2 m_WorkSpace;        //用于内部的计算







    protected override void Awake()
    {
        base.Awake();

        Rigidbody2d = GetComponentInParent<Rigidbody2D>();

        if (Rigidbody2d == null)
        {
            Debug.LogError("Rigidbody is missing in: " + name);
            return;
        }
    }




    //public override void LogicUpdate() { }

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



    public int GetFlipNum(Vector2 faceDirection, Vector2 currentDirection)      //如果不需要减去当前坐标，则第二个参数用Vector2.Zero
    {
        if (faceDirection != null)
        {
            Vector2 direction = (faceDirection - currentDirection).normalized;      //只需要方向

            int facingNum = direction.x < 0 ? -1 : 1;     //如果目标坐标位于当前坐标左侧，则翻转
            return facingNum;
        }
        return 0;
    }
}
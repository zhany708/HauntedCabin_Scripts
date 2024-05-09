using UnityEngine;



public class Movement : CoreComponent   //���ڹ����ƶ�
{
    public Rigidbody2D Rigidbody2d {  get; private set; }

    public Vector2 FacingDirection { get; private set; }


    Vector2 m_WorkSpace;        //�����ڲ��ļ���







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
            FacingDirection = m_WorkSpace.normalized;   //���ý�ɫ�ĳ�����
        }       
    }



    public int GetFlipNum(Vector2 faceDirection, Vector2 currentDirection)      //�������Ҫ��ȥ��ǰ���꣬��ڶ���������Vector2.Zero
    {
        if (faceDirection != null)
        {
            Vector2 direction = (faceDirection - currentDirection).normalized;      //ֻ��Ҫ����

            int facingNum = direction.x < 0 ? -1 : 1;     //���Ŀ������λ�ڵ�ǰ������࣬��ת
            return facingNum;
        }
        return 0;
    }
}
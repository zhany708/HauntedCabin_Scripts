using UnityEngine;


namespace ZhangYu.Utilities
{
    public class Flip
    {
        Transform m_Transform;

        //int m_FlipNumX;




        public Flip(Transform transform)
        {
            m_Transform = transform;
        }


        public void FlipX(int flipNum)
        {
            //m_FlipNumX = flipNum;

            //用于翻转物体的X值
            m_Transform.localScale = new Vector3(Mathf.Abs(m_Transform.localScale.x) * flipNum, m_Transform.localScale.y, m_Transform.localScale.z);      
        }

        public void FlipY(int flipNum)
        {
            //用于翻转物体的Y值
            m_Transform.localScale = new Vector3(m_Transform.localScale.x, Mathf.Abs(m_Transform.localScale.y) * flipNum, m_Transform.localScale.z);      
        }
    }
}
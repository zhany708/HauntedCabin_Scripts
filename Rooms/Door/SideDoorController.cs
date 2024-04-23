using System.Collections.Generic;
using UnityEngine;


public class SideDoorController : MonoBehaviour
{
    //��ɫ����ʱ��Ҫ����ֵ�����ڱ�ʾ��Ҫ���͵ķ��������ƫ��
    public float XOffset = 17f;
    public float YOffset = 10.7f;

    //��������ʱ͸���ȵ�ֵ
    public float HiddenTransparency = 0.5f;



    protected SpriteRenderer sprite;

    //���ڴ������д������ŵ���ײ��
    List<Collider2D> m_AllObjects;

    const float m_DefaultTransparency = 1f;






    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();     
    }

    private void Start()
    {
        //��ʼ��
        m_AllObjects = new List<Collider2D>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //ֻ����һ���˴������ŵĴ������󣬲Żή��͸����
        if (other.CompareTag("Player") || other.CompareTag("Enemy") )
        {
            m_AllObjects.Add(other);

            //�����ŵ�͸����
            ChangeTransparency(HiddenTransparency);
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        //�ȼ���뿪��ײ�����Ƿ�Ϊ��һ����
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            //�ټ���б��Ԫ�������Ƿ�Ϊ0
            if (m_AllObjects.Count > 0)
            {
                m_AllObjects.Remove(other);
            }

            //ֻҪ��Ȼ�д�����û���뿪�ţ���ô��ʹ���/�����뿪���ŵĴ�����������Ȼ���ְ�͸��
            if (m_AllObjects.Count == 0 && sprite.color.a == HiddenTransparency)        //ֻ���ŵ�͸����Ϊ�˽ű��еı���ʱ���ŵ���͸����
            {
                ChangeTransparency(m_DefaultTransparency);
            }
        }
    }




    private void ChangeTransparency(float alphaVal)
    {
        if (sprite != null)
        {
            //�����ŵ�͸����
            sprite.color = new Color(1f, 1f, 1f, alphaVal);
        }
    }
}
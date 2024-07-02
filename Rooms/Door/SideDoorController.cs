using System.Collections.Generic;
using UnityEngine;


public class SideDoorController : MonoBehaviour
{
    //房间门隐藏时透明度的值
    public float HiddenTransparency = 0.5f;



    protected SpriteRenderer sprite;



    //用于储存所有触发了门的碰撞器
    List<Collider2D> m_AllObjects = new List<Collider2D>();

    const float m_DefaultTransparency = 1f;     //默认透明度








    #region Unity内部函数
    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();     
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //只有玩家或敌人触发了门的触发器后，才会降低透明度
        if (other.CompareTag("Player") || other.CompareTag("Enemy") )
        {
            //检查碰撞器是否为空，防止Bug
            if (other != null)
            {
                m_AllObjects.Add(other);

                //降低门的透明度
                ChangeTransparency(HiddenTransparency);
            }          
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        //先检查离开碰撞器的是否为玩家或敌人
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            //再检查列表的元素数量是否为0
            if (m_AllObjects.Count > 0)
            {
                m_AllObjects.Remove(other);
            }

            //只要仍然有触发器没有离开门，那么即使玩家/敌人离开了门的触发器，门依然保持半透明
            if (m_AllObjects.Count == 0 && sprite.color.a == HiddenTransparency)        //只有门的透明度为此脚本中的变量时，才调回透明度
            {
                ChangeTransparency(m_DefaultTransparency);
            }
        }
    }
    #endregion


    #region 主要函数
    private void ChangeTransparency(float alphaVal)
    {
        if (sprite != null)
        {
            //更改门的透明度
            sprite.color = new Color(1f, 1f, 1f, alphaVal);
        }
    }
    #endregion
}
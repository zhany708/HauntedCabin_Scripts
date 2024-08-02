using UnityEngine;



//适合存放通用的死亡逻辑（玩家，敌人都有的逻辑）
public class Death : CoreComponent      //如果需要不同的死亡效果，则新建一个脚本，然后继承此脚本
{
    #region Unity内部函数
    private void OnEnable()
    {
        if (!combat.gameObject.activeSelf)
        {
            //重新激活战斗脚本组件（不是整个物体），防止再次加载后无法攻击敌人
            combat.enabled = true;
        }

        stats.OnHealthZero += Die;    //将函数加进事件
    }

    private void OnDisable()
    {
        stats.OnHealthZero -= Die;    //物体禁用后从事件中移除函数，防止因为找不到函数所在的脚本而报错
    }
    #endregion


    #region 其余函数
    private void Die()
    {
        //core.transform.parent.gameObject.SetActive(false);  //禁用游戏物体

        if (movement == null)
        {
            Debug.LogError("Cannot get the Movement component in the parent of:" + gameObject.name);
            return;
        }


        movement.Rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;        //死亡后禁止物体的一切移动和旋转
                                    
        combat.enabled = false;                     //取消激活战斗组件，防止出现鞭尸现象

        core.Animator.SetBool("Death", true);
    }
    #endregion
}
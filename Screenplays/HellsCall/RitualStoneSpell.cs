using System.Collections.Generic;
using UnityEngine;



public class RitualStoneSpell : MonoBehaviour       //祷告石护肤
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //只有当玩家身上没有护符时才会触发效果
        if (other.gameObject.CompareTag("Player") && !HellsCall.Instance.GetCanStartRitual() )
        {
            //UIManager.Instance.OpenInteractPanel(() => PickupLogic());     //打开互动面板

            PickupLogic();
        }
    }

    /*
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !InteractPanel.Instance.isRemoved)        //只有在界面打开时才关闭界面
        {
            //UIManager.Instance.ClosePanel(UIManager.Instance.UIKeys.InteractPanel);      //关闭互动界面
        }      
    }
    */



    private void PickupLogic()      //玩家拾取护符时的逻辑
    {
        //调整剧本物体中的布尔参数，表示玩家可以进行仪式
        HellsCall.Instance.SetCanStartRitual(true);

        //尝试获取地狱的呼唤脚本中储存祷告石的列表
        List<Vector2> allStonePos = HellsCall.Instance.GetAllStonePosList();
        if (allStonePos == null)
        {
            Debug.LogError("Cannot get the m_AllStonePos variable in the " + HellsCall.Instance.gameObject.name);
            return;
        }

        //获取完引用后检查列表中是否有当前祷告石的坐标，有的话则删除
        if (allStonePos.Contains(transform.position) )
        {
            allStonePos.Remove(transform.position);
        }

  
        Destroy(gameObject);        //删除祷告石护符物体
    }
}
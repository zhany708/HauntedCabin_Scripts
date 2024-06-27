using System.Collections.Generic;
using UnityEngine;



public class RitualStoneSpell : MonoBehaviour       //祷告石护肤
{





    private void Start()
    {
        //在这里加进字典，防止字典还没实例化就尝试获取引用导致报错
        if (!HellsCall.Instance.AllStonePosDict.ContainsKey(transform.position))
        {
            HellsCall.Instance.AllStonePosDict.Add(transform.position, gameObject);
        }
    }



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


        //从地狱的呼唤剧本脚本的字典中删除坐标
        if (HellsCall.Instance.AllStonePosDict.ContainsKey(transform.position))
        {
            HellsCall.Instance.AllStonePosDict.Remove(transform.position);
        }

        Destroy(gameObject);        //删除祷告石护符物体
    }
}
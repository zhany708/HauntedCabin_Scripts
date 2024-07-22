using UnityEngine;



public class DarkEvent : Event
{
    public int DarkEventIndex = -1;      //预兆事件的序列号，用于决定开启哪个剧本


    public static int DarkEventCount { get; private set; } = 0;   //表示玩家触发的预兆事件数量








    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();

        if (DarkEventIndex < 0)
        {
            Debug.LogError("DarkEventIndex is not assigned in the: " + gameObject.name);
            return;
        }
    }
    #endregion


    #region 事件相关
    protected override void FinishEvent()
    {       
        DarkEventCount++;       //增加触发的预兆事件计数

        base.FinishEvent();
    }
    #endregion


    #region 其余函数
    //重置游戏
    public static void ResetGame()
    {
        DarkEventCount = 0;
    }
    #endregion
}
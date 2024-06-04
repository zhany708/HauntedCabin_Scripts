public abstract class DarkEvent : Event
{
    protected static int darkEventCount = 0;   //表示玩家触发的预兆事件数量




    private void OnEnable()
    {
        darkEventCount++;       //增加预兆事件数
    }
}
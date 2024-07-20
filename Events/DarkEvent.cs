public abstract class DarkEvent : Event
{
    public int DarkEventIndex;      //预兆事件的序列号，用于决定开启哪个剧本


    protected static int darkEventCount = 0;   //表示玩家触发的预兆事件数量




    protected override void Awake()
    {
        base.Awake();

        if (DarkEventIndex == null)
        {
            Debug.LogError("DarkEventIndex is not assigned in the: " + gameObject.name);
            return;
        }
    }

    private void OnEnable()
    {
        darkEventCount++;       //增加预兆事件数
    }
}
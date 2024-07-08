using UnityEngine;




public class RitualRoom : RootRoomController        //仪式房脚本
{
    public static RitualRoom Instance { get; private set; }



    AltarHealthBarPanel m_AltarHealthBar;       //祷告石血条的引用



    protected override void Awake()
    {      
        //单例模式
        if (Instance != null && Instance != this)
        {
            //当重复生成仪式房时，删除重复的，同时生成通用房间以代替
            Destroy(gameObject);
          
            RoomManager.Instance.GenerateRoomAtThisPos(transform.position, RoomManager.Instance.RoomKeys.GenericRoomKey);
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        base.Awake();



        //获取祷告石血条UI界面组件
        m_AltarHealthBar = GetComponentInChildren<AltarHealthBarPanel>();
        if (m_AltarHealthBar == null)
        {
            Debug.LogError("AltarHealthBarPanel component not found in the children of: " + name);
            return;
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        //因为仪式房只有一个，所以生成后将仪式房从Key中移除
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(HellsCall.RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(HellsCall.RitualRoomName);
        }

        HellsCall.Instance.SetRitualRoomDoorController(DoorControllerInsideThisRoom);   //将仪式房的门控制器脚本传给剧本脚本
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        //玩家进入仪式房后将祷告石的血条显示出来
        if (other.CompareTag("Player"))
        {
            //设置界面的透明度（显示出来）
            m_AltarHealthBar.CanvasGroup.alpha = m_AltarHealthBar.FadeInAlpha;
        }        
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        //玩家离开仪式房后将祷告石的血条隐藏
        if (other.CompareTag("Player"))
        {
            //设置界面的透明度（隐藏）
            m_AltarHealthBar.CanvasGroup.alpha = m_AltarHealthBar.FadeOutAlpha;
        }       
    }
}
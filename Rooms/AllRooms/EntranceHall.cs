using UnityEngine;



public class EntranceHall : RootRoomController      //入口大堂脚本
{
    public static EntranceHall Instance { get; private set; }

    public GameObject ArrowTipObject;          //箭头提示，用于放在门前提醒玩家往此处移动




    #region Unity内部函数
    protected override void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            //当重复生成入口大堂时，删除重复的
            Destroy(gameObject);
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


        if (ArrowTipObject == null)
        {
            Debug.LogError("ArrowTipObject is not assigned correctly in the " + gameObject.name);
            return;
        }
    }

    protected override void Start()
    {
        //检查该房间是否是唯一保留的那个
        if (Instance == this)
        {
            base.Start();           //将房间加进RoomManager的GeneratedRoomDict字典和ImportantRoomPos列表
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);


        //当大门允许打开时，在玩家进入入口大堂后再打开大门
        if (other.CompareTag("Player") && MainDoorController.Instance.DoOpenMainDoor)   
        {
            MainDoorController.Instance.OpenMainDoor();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        //如果《箭头提示》物体处于激活状态，则取消激活
        if (ArrowTipObject.activeSelf)
        {
            ArrowTipObject.SetActive(false);
        }
    }

    /*
    protected override void OnDisable()
    {
        //只有含Instance的本房间取消激活时，才会从字典中移除当前房间的坐标
        if (Instance == this)
        {
            //暂时先不给入口大堂从字典中移除的机会，防止小地图无法正常显示
            //base.OnDisable();
        }
    }
    */
    #endregion


    #region 其它函数
    public override void ResetGame()
    {
        //检查该房间是否是唯一保留的那个
        if (Instance == this)
        {
            base.ResetGame();

            MainDoorController.Instance.CloseMainDoor();                //重置游戏时关闭大门
            MainDoorController.Instance.SetDoOpenMainDoor(false);       //关闭大门的同时重置布尔
        }
    }
    #endregion
}
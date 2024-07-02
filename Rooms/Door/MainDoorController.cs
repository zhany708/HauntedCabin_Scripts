using System;
using UnityEngine;



public class MainDoorController : MonoBehaviour        //用于大宅大门
{
    public static MainDoorController Instance { get; private set; }


    public Animator MainDoorAnimator { get; private set; }

    public bool DoOpenMainDoor { get; private set; } = false;      //表示是否打开大宅大门







    #region Unity内部函数
    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
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


        MainDoorAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        CloseMainDoor();        //游戏开始时关闭大门
    }


    private async void OnTriggerEnter2D(Collider2D other)     //需要做的：触发器不能太靠上，防止玩家横向经过的时候不小心触发了逻辑
    {
        if (other.CompareTag("Player"))     //玩家离开大宅后
        {
            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCall_GameWinningPanel);      //打开剧本胜利界面
        }
    }
    #endregion


    #region 开/关门相关的函数
    public void OpenMainDoor()       //打开大门
    {
        MainDoorAnimator.SetBool("isOpen", true);
        MainDoorAnimator.SetBool("isClose", false);
    }

    public void CloseMainDoor()     //关闭大门
    {
        MainDoorAnimator.SetBool("isOpen", false);
        MainDoorAnimator.SetBool("isClose", true);
    }
    #endregion


    #region Setters
    public void SetDoOpenMainDoor(bool isTrue)
    {
        DoOpenMainDoor = isTrue;
    }
    #endregion
}
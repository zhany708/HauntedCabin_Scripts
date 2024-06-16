using UnityEngine;


public class MainDoorController : MonoBehaviour        //用于大宅大门
{
    public Animator MainDoorAnimator { get; private set; }

    public bool DoOpenMainDoor { get; private set; } = false;      //表示是否打开大宅大门





    protected override void Awake()
    {
        MainDoorAnimator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)     //需要做的：触发器不能太靠上，防止玩家横向经过的时候不小心触发了逻辑
    {
        if (other.CompareTag("Player"))     //玩家离开大宅后
        {
            UIManager.Instance.OpenPanel(UIManager.UIKeys.HellsCall_GameWinningPanel);      //打开剧本胜利界面
        }
    }




    public void OpenMainDoor()       //打开大门
    {
        MainDoorAnimator.SetBool("isOpen", true);
        MainDoorAnimator.SetBool("isClose", false);
    }


    #region Setters
    public static void SetDoOpenMainDoor(bool isTrue)
    {
        DoOpenMainDoor = isTrue;
    }
    #endregion
}
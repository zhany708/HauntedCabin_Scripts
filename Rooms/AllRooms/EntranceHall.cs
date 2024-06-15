public class EntranceHall : RootRoomController      //入口大堂脚本
{
    public Animator MainDoorAnimator;




    public static void OpenMainDoor()       //打开大门
    {
        MainDoorAnimator.SetBool("isOpen", true);
        MainDoorAnimator.SetBool("isClose", false);
    }
}
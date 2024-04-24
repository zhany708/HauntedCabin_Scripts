


public class AllManagers : ManagerTemplate<AllManagers>
{
    protected override void Awake()
    {
        base.Awake();


        //将两个在主菜单界面就初始化过的管理器设置为自己的子物体
        UIManager.Instance.SetParent(transform);
        SoundManager.Instance.SetParent(transform);
    }
}



public class AllManagers : ManagerTemplate<AllManagers>
{
    protected override void Awake()
    {
        base.Awake();


        //�����������˵�����ͳ�ʼ�����Ĺ���������Ϊ�Լ���������
        UIManager.Instance.SetParent(transform);
        SoundManager.Instance.SetParent(transform);
    }
}
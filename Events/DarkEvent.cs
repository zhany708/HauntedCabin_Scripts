public abstract class DarkEvent : Event
{
    protected static int darkEventCount = 0;   //��ʾ��Ҵ�����Ԥ���¼�����




    private void OnEnable()
    {
        darkEventCount++;       //����Ԥ���¼���
    }
}
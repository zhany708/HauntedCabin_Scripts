using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //ר�������а�ť�Ľ���UI
{
    //���ڱ�֤һֱ����ʹ�ü��̲ٿذ�ť
    protected GameObject lastSelectedButton;


    protected virtual void Start()
    {
        if (lastSelectedButton != null)
        {
            //��ʼ����ť����������õ�EventSystem
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }        
    }


    protected virtual void Update()
    {
        //�����Ϊ�������ԭ�򣬵����޷��ü���ѡ��ť�����������ü���Ĭ�ϰ�ť
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }

        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }
}
using UnityEngine;



public class BaseScreenplay<T> : MonoBehaviour where T : Component         //���о籾�ĵײ��߼�����ͬ�㣩
{
    public static T Instance { get; private set; }





    protected virtual void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this as T)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this as T;

            //ֻ����û�и�����ʱ�����з�ɾ������������������
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    private void Start()
    {
        StartScreenplay();      //��ʼ�籾Setup
    }


    public virtual void StartScreenplay()       //�籾��ʼ���籾��Setup����������һЩ�����ȣ�
    {

    }



    private void Victory()     //ʤ����ص��߼�
    {

    }

    private void Lose()        //ʧ����ص��߼�
    {

    }
}
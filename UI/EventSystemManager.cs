using UnityEngine;



public class EventSystemManager : MonoBehaviour
{
    public static EventSystemManager Instance { get; private set; }




    private void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //ֻ����û�и�����ʱ�����з�ɾ������������������
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
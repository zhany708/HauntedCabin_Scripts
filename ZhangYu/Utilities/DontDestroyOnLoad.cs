using UnityEngine;



public class DontDestroyOnLoad : MonoBehaviour      //�������ڼ��س���ʱ��ֹɾ��������ʹ��
{
    //public bool IsSingleton;


    private void Awake()
    {
        if (transform.parent == null)
        {
            DontDestroyOnLoad(this);
        }        
    }
}
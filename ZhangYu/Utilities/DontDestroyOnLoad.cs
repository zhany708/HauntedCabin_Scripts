using UnityEngine;



public class DontDestroyOnLoad : MonoBehaviour      //�������ڼ��س���ʱ��ֹɾ��������ʹ��
{
    private void Awake()
    {
        if (transform.parent == null)
        {
            DontDestroyOnLoad(this);
        }        
    }
}
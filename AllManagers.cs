using UnityEngine;

public class AllManagers : MonoBehaviour
{
    private void Awake()
    {
        //ȷ���Լ��Լ����������岻���ڼ�����������ʱ��Ϊ���ⱻɾ��
        DontDestroyOnLoad(gameObject);      
    }
}

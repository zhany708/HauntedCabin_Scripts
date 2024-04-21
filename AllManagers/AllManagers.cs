using UnityEngine;

public class AllManagers : MonoBehaviour
{
    public static AllManagers Instance;




    private void Awake()
    {
        //����ģʽ
        if (Instance == null)
        {
            Instance = this;

            //ȷ���Լ��Լ����������岻���ڼ�����������ʱ��Ϊ���ⱻɾ��
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }          
    }
}
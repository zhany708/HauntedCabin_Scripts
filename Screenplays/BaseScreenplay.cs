using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BaseScreenplay : MonoBehaviour         //���о籾�ĵײ��߼�����ͬ�㣩
{





    private void Awake()
    {
        DontDestroyOnLoad(gameObject);      //���س���ʱ��ֹɾ���籾����
    }




    public virtual void StartScreenplay()       //�籾��ʼ
    {

    }



    private void Victory()     //ʤ����ص��߼�
    {

    }

    private void Lose()        //ʧ����ص��߼�
    {

    }
}
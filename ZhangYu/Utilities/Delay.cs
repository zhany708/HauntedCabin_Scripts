using System;
using System.Collections;
using UnityEngine;




public class Delay : MonoBehaviour      //���ڴ����ӳ���صĽű�
{
    public static Delay Instance { get; private set; }





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
        }
    }



    public IEnumerator DelaySomeTime(float delay, Action onTimerDone = null)      //�����ӳ�һ��ʱ���ִ��һЩ�߼�
    {
        yield return new WaitForSeconds(delay);

        onTimerDone?.Invoke();
    }



    //�ȴ���Ұ��ո�����
    public IEnumerator WaitForPlayerInput(Action onInputReceived = null)      
    {
        bool inputReceived = false;     //��ʾ�Ƿ���ܵ���ҵ��źţ����ھ����Ƿ����ѭ��

        while (!inputReceived)
        {
            //�������Ƿ��¿ո����������
            if (PlayerInputHandler.Instance.IsSpacePressed || PlayerInputHandler.Instance.AttackInputs[(int)CombatInputs.primary])
            {
                inputReceived = true;

                //�ȴ�0.15���ٵ��ûص�����������˺������������һ������Ҳ��Ҫ���ո�ʱ�����ܻᵼ�°�һ�οո���Ӧ�������
                yield return new WaitForSeconds(0.15f);

                onInputReceived?.Invoke();

                yield break;
            }

            yield return null;  //�ȴ�����һ֡Ϊֹ���Ӷ��ٴμ��
        }
    }
}
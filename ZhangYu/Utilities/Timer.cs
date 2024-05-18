using System;
using System.Collections;
using UnityEngine;




namespace ZhangYu.Utilities     //�����ļ��������Ժ�������Ϸ�����ܻ��õ��ĺ��������ʱ����
{
    public class Timer
    {
        public event Action OnTimerDone;    //���ն�����Weapon, EnemyFSM



        float m_StartTime;      //��ʼʱ��
        float m_Duration;       //��ʱʱ��
        float m_TargetTime;     //����ʱ��

        bool m_IsActive;                //��ʾ��ʱ���Ƿ񼤻���ڼ�ʱ��
        bool m_IsTimerDone = false;     //��ʾʱ���Ƿ��Ѿ����ˣ�����Э�̵ļ�ʱ��




        public Timer(float duration)
        {
            m_Duration = duration;
        }



        public void StartTimer()    //��ʼ��ʱ��
        {
            m_StartTime = Time.time;
            m_TargetTime = m_StartTime + m_Duration;
            m_IsActive = true;
            //m_IsTimerDone = false;
        }

        public void StopTimer()     //��ͣ��ʱ��
        {
            m_IsActive = false;
        }


        
        public void Tick()      //ʹ��ϵͳ�Դ���ʱ���ʱ����;������ͣ
        {
            if (!m_IsActive) return;


            if (Time.time > m_TargetTime)
            {
                //Debug.Log("Time up!");

                OnTimerDone?.Invoke();
                //m_IsTimerDone = true;
                StopTimer();    //����Ŀ��ʱ���ֹͣ��ʱ
            }
        }


        public IEnumerator WaitForDuration()        //ʹ��Э�̼�ʱ����;�޷���ͣ
        {
            m_IsTimerDone = false;

            yield return new WaitForSeconds(m_Duration);        //�ȴ�һ��ʱ��

            //Debug.Log("Time up!");
            m_IsTimerDone = true;
        }






        #region Getters
        public bool GetIsTimerDone()
        {
            return m_IsTimerDone;
        }
        #endregion
    }
}
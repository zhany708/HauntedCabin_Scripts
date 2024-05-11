using System;
using UnityEngine;




namespace ZhangYu.Utilities     //�����ļ��������Ժ�������Ϸ�����ܻ��õ��ĺ��������ʱ����
{
    public class Timer
    {
        public event Action OnTimerDone;    //���ն�����Weapon, EnemyFSM



        float m_StartTime;      //��ʼʱ��
        float m_Duration;       //��ʱʱ��
        float m_TargetTime;     //����ʱ��

        bool m_IsActive;        //��ʾ��ʱ���Ƿ񼤻���ڼ�ʱ��
            




        public Timer(float duration)
        {
            m_Duration = duration;
        }



        public void StartTimer()    //��ʼ��ʱ��
        {
            m_StartTime = Time.time;
            m_TargetTime = m_StartTime + m_Duration;
            m_IsActive = true;
        }

        public void StopTimer()     //��ͣ��ʱ��
        {
            m_IsActive = false;
        }


        
        public void Tick()      //������ʱ
        {
            if (!m_IsActive) return;


            if (Time.time > m_TargetTime)
            {
                //Debug.Log("Time up!");

                OnTimerDone?.Invoke();
                StopTimer();    //����Ŀ��ʱ���ֹͣ��ʱ
            }
        }
    }
}
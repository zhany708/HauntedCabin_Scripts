using System;
using UnityEngine;

namespace ZhangYu.Utilities     //张煜文件夹用于以后所有游戏都可能会用到的函数，如计时器等
{
    public class Timer
    {
        public event Action OnTimerDone;    //接收对象有Weapon, EnemyFSM

        float m_StartTime;
        float m_Duration;
        float m_TargetTime;

        bool m_IsActive;

        public Timer(float duration)
        {
            m_Duration = duration;
        }



        public void StartTimer()
        {
            m_StartTime = Time.time;
            m_TargetTime = m_StartTime + m_Duration;
            m_IsActive = true;
        }

        public void StopTimer()
        {
            m_IsActive = false;
        }


        
        public void Tick()
        {
            if (!m_IsActive) return;


            if (Time.time > m_TargetTime)
            {
                //Debug.Log("Time up!");

                OnTimerDone?.Invoke();
                StopTimer();    //到达目标时间后停止计时
            }
        }
    }
}

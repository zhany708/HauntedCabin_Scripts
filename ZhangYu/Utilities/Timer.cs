using System;
using UnityEngine;




namespace ZhangYu.Utilities     //张煜文件夹用于以后所有游戏都可能会用到的函数，如计时器等
{
    public class Timer
    {
        public event Action OnTimerDone;    //接收对象有Weapon, EnemyFSM



        float m_StartTime;      //开始时间
        float m_Duration;       //计时时长
        float m_TargetTime;     //结束时间

        bool m_IsActive;        //表示计时器是否激活（正在计时）
            




        public Timer(float duration)
        {
            m_Duration = duration;
        }



        public void StartTimer()    //开始计时器
        {
            m_StartTime = Time.time;
            m_TargetTime = m_StartTime + m_Duration;
            m_IsActive = true;
        }

        public void StopTimer()     //暂停计时器
        {
            m_IsActive = false;
        }


        
        public void Tick()      //持续计时
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
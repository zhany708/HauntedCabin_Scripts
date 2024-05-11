using System.Collections.Generic;
using UnityEngine;



namespace ZhangYu.Utilities     //张煜文件夹用于以后所有游戏都可能会用到的函数，如计时器，生成随机坐标等
{
    public class RandomPosition
    {
        Vector2 m_LeftDownPosition;     //用于随机生成巡逻坐标
        Vector2 m_RightTopPosition;
        float m_OverlapTolerance = 1f;     //检查重复坐标时决定坐标间是否重复的距离值，默认1

        public RandomPosition(Vector2 leftDownPos, Vector2 rightTopPos, float overlapTolerance)
        {
            m_LeftDownPosition = leftDownPos;
            m_RightTopPosition = rightTopPos;
            m_OverlapTolerance = overlapTolerance;
        }





        public Vector2 GenerateSingleRandomPos()
        {
            //根据左下角的坐标和右上角的坐标，在一个长方形内随机生成坐标
            return new Vector2(Random.Range(m_LeftDownPosition.x, m_RightTopPosition.x), Random.Range(m_LeftDownPosition.y, m_RightTopPosition.y));
        }


        //此函数在生成单个坐标的同时，还保证不会跟参数中列表里的所有坐标重复
        public Vector2 GenerateNonOverlappingPosition(List<Vector2> existingPositions, int maxAttempts = 100)
        {
            Vector2 position;
            int attempts = 0;

            do
            {
                position = GenerateSingleRandomPos();

                if (++attempts > maxAttempts)       //尝试过100次后，则报错
                {
                    Debug.LogError("Failed to generate non-overlapping position after " + maxAttempts + " attempts.");
                    break;
                }
            }
            while (CheckOverlapForSinglePosition(existingPositions, position));

            return position;
        }



        //生成多个坐标，数量为参数中的变量，最后返回一个列表
        public List<Vector2> GenerateMultiRandomPos(int num)
        {
            //Debug.Log("Number of position that need to generate is " + num);

            List<Vector2> newPos = new List<Vector2>();

            //如果参数小于等于0，则返回空列表
            if (num <= 0)  return newPos;


            //将所有坐标生成出来，且这些坐标不会重复
            for (int i = 0; i < num; i++)
            {
                newPos.Add(GenerateNonOverlappingPosition(newPos));
            }

            return newPos;         
        }

 


        //检查参数中的坐标是否跟参数中的列表里的任意一个坐标重合
        private bool CheckOverlapForSinglePosition(List<Vector2> positions, Vector2 candidatePosition)
        {
            foreach (Vector2 existingPosition in positions)
            {
                if (IsOverlap(candidatePosition, existingPosition))
                {
                    return true;
                }
            }

            return false;
        }



        private bool IsOverlap(Vector2 firstPos, Vector2 secondPos)     //检查两个坐标是否几乎重合
        {
            //如果两个坐标的x和y的差值都小于脚本中的检测变量，则视为重合
            return (Mathf.Abs(secondPos.x - firstPos.x) <= m_OverlapTolerance) && (Mathf.Abs(secondPos.y - firstPos.y) <= m_OverlapTolerance);
        }



        #region Setters
        public void SetOverlapTolerance(float newTolerance)
        {
            m_OverlapTolerance = newTolerance;
        }
        #endregion

        #region Getters
        public float GetOverlapTolerance()
        {
            return m_OverlapTolerance;
        }
        /*
        public Vector2 GetLeftDownPos()
        {
            return m_LeftDownPosition;
        }

        public Vector2 GetRightTopPos()
        {
            return m_RightTopPosition;
        }
        */
        #endregion
    }
}
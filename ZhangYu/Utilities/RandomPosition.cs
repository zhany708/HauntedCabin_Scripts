using System.Collections.Generic;
using UnityEngine;



namespace ZhangYu.Utilities     //�����ļ��������Ժ�������Ϸ�����ܻ��õ��ĺ��������ʱ����������������
{
    public class RandomPosition
    {
        Vector2 m_LeftDownPosition;     //�����������Ѳ������
        Vector2 m_RightTopPosition;
        float m_OverlapTolerance = 1f;     //����ظ�����ʱ����������Ƿ��ظ��ľ���ֵ��Ĭ��1

        public RandomPosition(Vector2 leftDownPos, Vector2 rightTopPos, float overlapTolerance)
        {
            m_LeftDownPosition = leftDownPos;
            m_RightTopPosition = rightTopPos;
            m_OverlapTolerance = overlapTolerance;
        }





        public Vector2 GenerateSingleRandomPos()
        {
            //�������½ǵ���������Ͻǵ����꣬��һ���������������������
            return new Vector2(Random.Range(m_LeftDownPosition.x, m_RightTopPosition.x), Random.Range(m_LeftDownPosition.y, m_RightTopPosition.y));
        }


        //�˺��������ɵ��������ͬʱ������֤������������б�������������ظ�
        public Vector2 GenerateNonOverlappingPosition(List<Vector2> existingPositions, int maxAttempts = 100)
        {
            Vector2 position;
            int attempts = 0;

            do
            {
                position = GenerateSingleRandomPos();

                if (++attempts > maxAttempts)       //���Թ�100�κ��򱨴�
                {
                    Debug.LogError("Failed to generate non-overlapping position after " + maxAttempts + " attempts.");
                    break;
                }
            }
            while (CheckOverlapForSinglePosition(existingPositions, position));

            return position;
        }



        //���ɶ�����꣬����Ϊ�����еı�������󷵻�һ���б�
        public List<Vector2> GenerateMultiRandomPos(int num)
        {
            //Debug.Log("Number of position that need to generate is " + num);

            List<Vector2> newPos = new List<Vector2>();

            //�������С�ڵ���0���򷵻ؿ��б�
            if (num <= 0)  return newPos;


            //�������������ɳ���������Щ���겻���ظ�
            for (int i = 0; i < num; i++)
            {
                newPos.Add(GenerateNonOverlappingPosition(newPos));
            }

            return newPos;         
        }

 


        //�������е������Ƿ�������е��б��������һ�������غ�
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



        private bool IsOverlap(Vector2 firstPos, Vector2 secondPos)     //������������Ƿ񼸺��غ�
        {
            //������������x��y�Ĳ�ֵ��С�ڽű��еļ�����������Ϊ�غ�
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
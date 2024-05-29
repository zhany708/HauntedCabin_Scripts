using System.Collections.Generic;
using UnityEngine;



public class HellsCall : BaseScreenplay
{
    public GameObject RitualStone;      //����ʯ����


    public PlayerStats PlayerStats      //Lazy load
    {
        get
        {
            if (m_PlayerStats == null)
            {
                m_PlayerStats = FindObjectOfType<PlayerStats>();
            }
            return m_PlayerStats;
        }
    }
    private PlayerStats m_PlayerStats;



    Coroutine m_HealthDrainCoroutine;

    int m_StoneNum = 2;     //����ʯ������




    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //�������ʱֹͣЭ��
    }

    private void OnDisable()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnHealthZero -= DestroyCoroutine;
        }
    }

   



    public async override void StartScreenplay()
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //�򿪾籾��������
                                                                         
        GenerateRitualStones();     //���ɵ���ʯ
    }



   


    public void StartHealthDrain()      //��ʼ������Ѫ
    {
        if (PlayerStats != null)
        {
            //����10�룬ÿ�ε�5��Ѫ��ÿ5���һ��
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(10f, 5f, 12f));
        }
    }

    private void DestroyCoroutine()     //ֹͣЭ��
    {
        if (m_HealthDrainCoroutine != null)
        {
            UnityEngine.Debug.Log("Coroutine stopped!!");
            StopCoroutine(m_HealthDrainCoroutine);
        }
    }




    public void GenerateRitualStones()       //������������ɵ���ʯ
    {
        int roomNum = RoomManager.Instance.GeneratedRoomDict.Count;     //��ʾ��ǰ�ж��ٷ���

        if (roomNum <= m_StoneNum)      //���������������������е���ʯʱ
        {
            //��Ҫ���ģ��ں����������ɺ�ǿ�����ɵ���ʯ
        }

        else
        {
            List<Vector2> tempRoomPos = new List<Vector2>();    //���ڴ������з����ֵ��������

            // ���ֵ�����������괢�����б���
            foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
            {
                tempRoomPos.Add(room);
            }

            tempRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage() );   //�Ƴ�����������׶εķ�������꣬��ֹ������̻�õ���ʯ

            //������ĵ���ʯȫ�����ɳ���
            for (int i = 0; i < m_StoneNum; i++)
            {
                int randomNum = UnityEngine.Random.Range(0, tempRoomPos.Count); //�����������
                Vector2 selectedRoomPos = tempRoomPos[randomNum];               //��ȡ���ѡ��ķ��������
                tempRoomPos.RemoveAt(randomNum);                                //�Ƴ���ѡ��ķ����Է�ֹ�ظ�

                // ��ѡ�еķ������ɵ���ʯ
                EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[selectedRoomPos].transform, selectedRoomPos);
            }
        }
    }
}
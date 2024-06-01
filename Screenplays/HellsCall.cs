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

    bool m_NeedGenerateStone = false;   //�ж��Ƿ���Ҫ���ɵ���ʯ
    public int m_NeededStoneNum = 2;    //��Ҫ���ɵĵ���ʯ������
    int m_GeneratedStoneNum = 0;        //��ʾ��ǰ�����˶��ٵ���ʯ



    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //�������ʱֹͣЭ��
        RoomManager.Instance.OnRoomGenerated += GenerateStoneAtSingleRoom;      //�����ɷ���ʱ�����ô˺���
    }

    private void OnDisable()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnHealthZero -= DestroyCoroutine;
        }

        RoomManager.Instance.OnRoomGenerated += GenerateStoneAtSingleRoom;
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



    //�����������еĵ���ʯ�������籾ֻ����һ�Σ�
    public void GenerateRitualStones()       //������������ɵ���ʯ
    {
        List<Vector2> tempRoomPos = new List<Vector2>();    //���ڴ������з����ֵ��������

        //���ֵ�����������괢�����б���
        foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
        {
            tempRoomPos.Add(room);
        }

        tempRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage());   //�Ƴ�����������׶εķ�������꣬��ֹ������̻�õ���ʯ


        //�жϷ��������Ƿ��㹻�������е���ʯ
        if (tempRoomPos.Count <= m_NeededStoneNum)      //���������������������е���ʯʱ
        {
            //��Ҫ���ģ��ں����������ɺ�ǿ�����ɵ���ʯ
            GenerateSeveralStones(tempRoomPos.Count, tempRoomPos);      //�����ɶ��ٵ���ʯ�������ɶ���

            m_NeedGenerateStone = true;
        }

        else
        {
            GenerateSeveralStones(m_NeededStoneNum, tempRoomPos);       //�������е���ʯ
        }
    }

    private void GenerateSeveralStones(int generatedNum, List<Vector2> roomPosList)     //���ɲ����е������ĵ���ʯ
    {
        //������ĵ���ʯȫ�����ɳ���
        for (int i = 0; i < generatedNum; i++)
        {
            int randomNum = UnityEngine.Random.Range(0, roomPosList.Count); //�����������
            Vector2 selectedRoomPos = roomPosList[randomNum];               //��ȡ���ѡ��ķ��������
            roomPosList.RemoveAt(randomNum);                                //�Ƴ���ѡ��ķ����Է�ֹ�ظ�

            //��ѡ�еķ������ɵ���ʯ
            EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[selectedRoomPos].transform, selectedRoomPos);

            m_GeneratedStoneNum++;      //���ӵ���ʯ����
        }
    }

    //�ڵ����ķ������ɵ���ʯ
    private void GenerateStoneAtSingleRoom(Vector2 roomPos)
    {
        if (m_NeedGenerateStone)
        {
            //�ڲ����еķ������ɵ���ʯ
            EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[roomPos].transform, roomPos);

            m_GeneratedStoneNum++;      //���ӵ���ʯ����
        }        

        if (m_GeneratedStoneNum >= m_NeededStoneNum)        //�ж��Ƿ��Ѿ��������㹻�ĵ���ʯ
        {
            m_NeedGenerateStone = false;
        }
    }
}
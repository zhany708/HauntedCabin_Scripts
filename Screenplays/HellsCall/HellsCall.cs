using System.Collections.Generic;
using UnityEngine;



public class HellsCall : BaseScreenplay<HellsCall>
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



    Coroutine m_HealthDrainCoroutine;       //��ҳ�����Ѫ��Э��

    List<Vector2> m_TempRoomPos = new List<Vector2>();    //���ڴ������з����ֵ��������

    string m_RitualRoomName = "RitualRoom";

    bool m_NeedGenerateStone = false;   //�ж��Ƿ���Ҫ���ɵ���ʯ
    bool m_CanStartRitual = false;      //�ж��Ƿ���Կ�ʼ��ʽ

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

    private void OnDestroy()
    {
        //����ʽ�������ִ��б����Ƴ�����ΪScriptObject�ļ�¼����������Ϸ��������ʧ
        RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(m_RitualRoomName);
    }



    public async override void StartScreenplay()
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //�򿪾籾��������
                                                                         
        GenerateRitualStones();     //���ɵ���ʯ                                                                                                                                                                                                                                                                                                                                                                                                   666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
        GenerateRitualRoom();       //������ʽ��
    }





    #region ��ҳ�����Ѫ���
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
    #endregion


    private void AddAllRoomPosIntoList()
    {
        //���ֵ�����������괢�����б���
        foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
        {
            m_TempRoomPos.Add(room);
        }

        m_TempRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage());   //�Ƴ�����������׶εķ�������꣬��ֹ������̻�õ���ʯ
    }
    


    #region ��ʽ�����
    private void GenerateRitualRoom()       //������ʽ����������ͼֻ��һ����
    {
        //һ�п������ɵķ���������FloorToInt�������ڽ��������ȡ��������С�������ж��
        int allowedRoomNumOnRow = Mathf.FloorToInt(RoomManager.Instance.MaximumXPos * 2 / RoomManager.RoomLength ) + 1;
        //һ�п������ɵķ�������
        int allowedRoomNumOnColumn = Mathf.FloorToInt(RoomManager.Instance.MaximumYPos * 2 / RoomManager.RoomWidth) + 1;

        int maxAllowedRoomNum = allowedRoomNumOnRow * allowedRoomNumOnColumn;       //һ¥�������ɵ���󷿼�������ǰΪ35��


        //��û���µķ����������ʱ
        if (RoomManager.Instance.GeneratedRoomDict.Count >= maxAllowedRoomNum)
        {
            AddAllRoomPosIntoList();

            Vector2 selectedRoomPos = GenerateSuitableRandomRoomPos();     //���ѡ��ķ��������
            GameObject deletedRoom = null;                                 //���������Ӧ�ķ���

            if (RoomManager.Instance.GeneratedRoomDict.TryGetValue(selectedRoomPos, out deletedRoom) )   //���Դ��ֵ��л�ȡ��Ӧ�ķ���
            {
                Destroy(deletedRoom);       //ɾ����������Ӧ�ķ��䣬�����ʽ������������
            }

            else
            {
                Debug.LogError("A room has generated here, but cannot get the corresponding gameobject: " + selectedRoomPos);
            }
        }

        else
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Add(m_RitualRoomName);       //����ʽ�������ּӽ��б����Ա������������
        }
    }

    private Vector2 GenerateSuitableRandomRoomPos()    //���ɺ��ʵ�����������꣨��ΪĳЩ���䲻�ɸ��ģ�
    {
        List<Vector2> importantRoomPos = new List<Vector2>();    //���ڴ������в��ɸ��ĵķ������꣨�����ʼ����ȣ�

        importantRoomPos.Add(Vector2.zero);     //����ڴ������ӽ��б�    ����Ҫ���ģ����������һ¥��ʼ���


        Vector2 selectedRoomPos = Vector2.zero;     //���ڴ������ѡ�񵽵ķ��������
        int attemptCount = 0;                       //��ʾ�����˶��ٴ�
        const int maxAttemptCount = 50;             //����Դ���

        //ֻҪ��������ɸ��ĵķ������꣬�����»�ȡ�������
        while (importantRoomPos.Contains(selectedRoomPos) && attemptCount <= maxAttemptCount)
        {
            int randomNum = Random.Range(0, m_TempRoomPos.Count);   //�����������
            selectedRoomPos = m_TempRoomPos[randomNum];     //��ȡ���ѡ��ķ��������

            attemptCount++;     //���ӳ��Լ���
        }

        if (attemptCount > maxAttemptCount)        //Report error if attempCount exceed the maximun allowed counts
        {
            Debug.LogError("Failed to generate a suitable random room position after " + maxAttemptCount + " attempts!");
        }


        return selectedRoomPos;
    }
    #endregion


    #region ����ʯ���
    //�����������еĵ���ʯ�������籾ֻ����һ�Σ�
    public void GenerateRitualStones()       //������������ɵ���ʯ
    {
        AddAllRoomPosIntoList();    //���ֵ�����������괢�����б���


        //�жϷ��������Ƿ��㹻�������е���ʯ
        if (m_TempRoomPos.Count <= m_NeededStoneNum)      //���������������������е���ʯʱ
        {
            //��Ҫ���ģ��ں����������ɺ�ǿ�����ɵ���ʯ
            GenerateSeveralStones(m_TempRoomPos.Count, m_TempRoomPos);      //�����ɶ��ٵ���ʯ�������ɶ���

            m_NeedGenerateStone = true;
        }

        else
        {
            GenerateSeveralStones(m_NeededStoneNum, m_TempRoomPos);       //�������е���ʯ
        }
    }

    private void GenerateSeveralStones(int generatedNum, List<Vector2> roomPosList)     //���ɲ����е������ĵ���ʯ
    {
        //������ĵ���ʯȫ�����ɳ���
        for (int i = 0; i < generatedNum; i++)
        {
            int randomNum = Random.Range(0, roomPosList.Count); //�����������
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
    #endregion


    #region Setters
    public void SetCanStartRitual(bool isTrue)
    {
        Debug.Log("Can player start the ritual?" + isTrue);

        m_CanStartRitual = isTrue;
    }
    #endregion


    #region Getters
    public bool GetCanStartRitual()
    {
        return m_CanStartRitual;
    }
    #endregion
}
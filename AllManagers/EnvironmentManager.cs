using System;
using UnityEngine;


//���ڴ�����Ϸ�����е�һЩ��̬�仯��������ĳ���ط�����ĳ���¶�����
public class EnvironmentManager : ManagerTemplate<EnvironmentManager>     
{
    public event Action OnEnemyKilled;       //���շ�ΪTaskPanel


    public GameObject BlockDoorBarrel;      //��ס��ҽ����ŵ��ϰ���

    public int KilledEnemyCount { get; private set; } = 0;     //��ʾɱ�������ٵ���
    public int RequiredEnemyCount { get; private set; } = 6;   //��ʾ��Ҫɱ�����ٵ��ˣ���Ϸ��ʤ��
    public bool IsGameOver { get; private set; } = false;



    







    protected override void Awake()
    {
        base.Awake();

        if (BlockDoorBarrel == null)
        {
            Debug.LogError("BlockDoorBarrel is not assigned in the EnvironmentManager.");
            return;
        }
    }





    //����ľͰ��������ֹ��Ҵ����ţ���������Ϸ�еĶ�̬�仯�Ž�һ��Manager�У��Ӷ��������ű�רע���������£�
    public void GenerateBarrelToBlockDoor(Transform parentTransform, Vector2 generatedPos)
    {
        Instantiate(BlockDoorBarrel, generatedPos, Quaternion.identity, parentTransform);

        //���ԴӸ����������ȡ�ű����
        RootRoomController parentObject = parentTransform.GetComponent<RootRoomController>();

        if (parentObject != null)
        {
            //����µľ���ͼ
            parentObject.AddNewSpriteRenderers();
        }
    }


    public async void IncrementKilledEnemyCount()
    {
        KilledEnemyCount++;

        OnEnemyKilled?.Invoke();        //���ûص�����

        if (KilledEnemyCount >= RequiredEnemyCount)     //����Ƿ�ɱ�����㹻�ĵ���
        {
            //Debug.Log("You win!");      //��Ҫ���ģ�����Ϸʤ�����棬�����˶�����е����е���ǿ�н�������״̬
            IsGameOver = true;

            EnemyPool.Instance.EndGame();       //�ڳ����������м���ĵ��˽�������״̬

            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameWinningPanel);
        }
    }
}
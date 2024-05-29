using System;
using System.Collections.Generic;
using UnityEngine;



//���ڴ�����Ϸ�����е�һЩ��̬�仯��������ĳ���ط�����ĳ���¶�����
public class EnvironmentManager : ManagerTemplate<EnvironmentManager>     
{
    public event Action OnEnemyKilled;       //���շ�ΪTaskPanel


    public int KilledEnemyCount { get; private set; } = 0;     //��ʾɱ�������ٵ���
    public int RequiredEnemyCount { get; private set; } = 6;   //��ʾ��Ҫɱ�����ٵ��ˣ���Ϸ��ʤ��
    public bool IsGameOver { get; private set; } = false;



    







    protected override void Awake()
    {
        base.Awake();
    }





    //�������壬ͬʱ�������е�Transform����Ϊ����ĸ�����     ��Ҫ���ģ��ö������������
    public void GenerateObjectWithParent(GameObject generatedObject, Transform parentTransform, Vector2 generatedPos)
    {
        Instantiate(generatedObject, generatedPos, Quaternion.identity, parentTransform);

        //���ԴӸ����������ȡ�ű����
        RootRoomController parentObject = parentTransform.GetComponent<RootRoomController>();

        if (parentObject != null)
        {
            //����µľ���ͼ
            parentObject.AddNewSpriteRenderers();
        }
    }


    #region ���ɵ������
    //���ɵ��˺���Physics2D.Oberlap�����Ｔ�����ɵ������Ƿ���Ҿ��غϣ�����غ���������������
    public void GenerateEnemy(DoorController doorController)
    {
        if (doorController.EnemyObjects.Length != 0)   //��������й���
        {
            List<Vector2> enemySpawnList = doorController.EnemySpwanPos.GenerateMultiRandomPos(doorController.EnemyObjects.Length);     //���ݹ������������������list

            //�����������б�󡣼���б����Ƿ��и��Ҿ��غϵ�����
            CheckIfCollideFurniture(enemySpawnList, doorController);



            for (int i = 0; i < doorController.EnemyObjects.Length; i++)
            {
                //�����enemy�����ǵ��˵ĸ����壨����Ѳ������ģ��������ɵ�ͬʱ����������������
                GameObject enemyObject = EnemyPool.Instance.GetObject(doorController.EnemyObjects[i], enemySpawnList[i]);     //�ӵ��˶���������ɵ���

                //Debug.Log("The enemy spawn position is : " + enemySpawnList[i]);

                //����������õ��˽ű��󶨵�����ı��أ�����ڸ����壩���ꡣ��Ϊ���˴Ӷ�����������ɺ󣬱��������̳�����ǰ�ı�������
                Enemy enemyScript = enemyObject.GetComponentInChildren<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.ResetLocalPos();
                }


                //�����ſ������Ľű�
                enemyScript.SetDoorController(doorController);

                //���ɵ��˺������������������¼���ĵ���������ȻΪ0
                enemyObject.GetComponentInChildren<Stats>().SetCurrentHealth(enemyObject.GetComponentInChildren<Stats>().MaxHealth);
            }
        }
    }


    //����б��е��������괦�Ƿ��мҾ�
    private void CheckIfCollideFurniture(List<Vector2> enemySpawnPosList, DoorController doorController)
    {
        Vector2 checkSize = new Vector2(doorController.PhysicsCheckingXPos, doorController.PhysicsCheckingYPos);      //������Ĵ�С

        float adaptiveTolerance = doorController.EnemySpwanPos.GetOverlapTolerance();        //��ȡ����ظ��ľ���
        int attemptCount = 0;       //���ڷ�ֹ��������ѭ���ı���


        while (attemptCount < 100)      //ȷ������������Դ���
        {
            bool isOverlap = false;

            for (int i = 0; i < enemySpawnPosList.Count; i++)
            {
                if (!IsPositionEmpty(enemySpawnPosList[i], checkSize, doorController))
                {
                    enemySpawnPosList[i] = doorController.EnemySpwanPos.GenerateNonOverlappingPosition(enemySpawnPosList);

                    doorController.EnemySpwanPos.SetOverlapTolerance(adaptiveTolerance);     //�����µļ���ظ��ľ���

                    isOverlap = true;  //���ò����Լ������
                }
            }

            if (!isOverlap) break;  //��û���ظ�ʱ���˳�ѭ��

            attemptCount++;
            adaptiveTolerance -= 0.1f;  //���ʵ���������ɲ����ظ�������Ļ������ټ���ظ��ľ���
        }

    }


    //�������������Ҫ���ɵ������Ƿ��мҾ�
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize, DoorController doorController)
    {
        //��һ������Ϊ���ĵ㣬�ڶ�������Ϊ�����δ�С���������ĸ�����һ�룩������������Ϊ�Ƕȣ����ĸ�����Ϊ����Ŀ��㼶
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, checkSize, 0f, doorController.FurnitureLayerMask);
        return overlapCheck == null;
    }
    #endregion


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
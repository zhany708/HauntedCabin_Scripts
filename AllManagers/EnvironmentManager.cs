using UnityEngine;


public class EnvironmentManager : MonoBehaviour     //���ڴ�����Ϸ�����е�һЩ��̬�仯��������ĳ���ط�����ĳ���¶�����
{
    public static EnvironmentManager Instance { get; private set; }


    public GameObject BlockDoorBarrel;      //��ס��ҽ����ŵ��ϰ���







    private void Awake()
    {
        //����ģʽ
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
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
}
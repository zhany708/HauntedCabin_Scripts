using UnityEngine;


public class EnvironmentManager : ManagerTemplate<EnvironmentManager>     //���ڴ�����Ϸ�����е�һЩ��̬�仯��������ĳ���ط�����ĳ���¶�����
{
    public GameObject BlockDoorBarrel;      //��ס��ҽ����ŵ��ϰ���







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
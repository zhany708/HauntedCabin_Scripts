using UnityEngine;



public class NewBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //ֻ�е��������û�л���ʱ�Żᴥ��Ч��
        if (other.gameObject.CompareTag("Player") && !HellsCall.Instance.GetCanStartRitual() )
        {
            //�����籾�����еĲ�����������ʾ��ҿ��Խ�����ʽ
            HellsCall.Instance.SetCanStartRitual(true);

            Destroy(gameObject);        //ɾ������ʯ��������
        }
    }
}

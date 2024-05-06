using UnityEngine;


//�ʺϴ��ͨ�õ������߼�����ң����˶��е��߼���
public class Death : CoreComponent      //�����Ҫ��ͬ������Ч�������½�һ���ű���Ȼ��̳д˽ű�
{



    private void OnEnable()
    {
        Stats.OnHealthZero += Die;    //�������ӽ��¼�
    }

    private void OnDisable()
    {
        Stats.OnHealthZero -= Die;    //������ú���¼����Ƴ���������ֹ��Ϊ�Ҳ����������ڵĽű�������
    }




    public virtual void Die()
    {
        //core.transform.parent.gameObject.SetActive(false);  //������Ϸ����

        Movement.Rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;        //��ֹ���������廹�ܽ����ƶ�

        //ȡ������ս���������ֹ���ֱ�ʬ����
        combat.gameObject.SetActive(false);

        core.Animator.SetBool("Death", true);
    }
}
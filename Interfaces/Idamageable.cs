using UnityEngine;



public interface Idamageable        //�������п��Ա��˺�������
{
    void Damage(float amount, bool doesIgnoreDefense);      //��������ֵ����һ��������ʾ�˺������ڶ���������ʾ�Ƿ����ӷ�����

    //void GetHit(Vector2 direction);     //�ܻ�˲��ִ�е��߼�������ת�򣬸ı���̬�ȣ�
}
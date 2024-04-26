using UnityEngine;


[CreateAssetMenu(fileName = "newUIKeys", menuName = "Data/UI Data/UI Keys")]
public class SO_UIKeys : ScriptableObject
{
    //�˵����
    [Header("Menu")]
    public string MainMenuPanel;              //��Ϸ��ʼ�˵�
    public string PauseMenuPanel;           //��Ϸ��ͣ����


    //�������
    [Header("Player")]
    public string PlayerStatusBarKey;         //���״̬������HealthBar�ű����ʼ��


    //�¼����
    [Header("Event")]
    public string TransitionStagePanelKey;    //������׶�����


    //�������
    [Header("Weapon")]
    public string PickupWeaponPanelKey;       //�������ʰȡ����  
}
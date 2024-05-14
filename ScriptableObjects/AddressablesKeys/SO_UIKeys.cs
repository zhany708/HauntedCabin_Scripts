using UnityEngine;


[CreateAssetMenu(fileName = "newUIKeys", menuName = "Data/UI Data/UI Keys")]
public class SO_UIKeys : ScriptableObject
{
    //�˵����
    [Header("Menu")]
    public string MainMenuPanel;        //��Ϸ��ʼ�˵�
    public string SettingPanel;         //��Ϸ���ý���
    public string PauseMenuPanel;       //��Ϸ��ͣ����
    public string GameLostPanel;        //��Ϸʧ�ܽ���
    public string GameWinningPanel;     //��Ϸʤ������

    public string TaskPanel;                //��Ϸ�������


    //�籾���
    [Header("ScreenPlay")]
    public string GameBackgroundPanel;      //��Ϸ�ײ�籾����
    


    //�������
    [Header("Player")]
    public string PlayerStatusBarKey;         //���״̬������HealthBar�ű����ʼ��


    //�¼����
    [Header("Event")]
    public string TransitionStagePanelKey;    //������׶�����
    public string EvilTelephonePanel;         //���绰�������¼�����


    //�������
    [Header("Weapon")]
    public string PickupWeaponPanelKey;       //�������ʰȡ����  
}
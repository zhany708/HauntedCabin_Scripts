using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public class PostProcessController : MonoBehaviour
{
    //������ɫ�˾����   
    public Color OrangeFilter = new Color(250, 107, 58);   //��ɫ
    public Color RedFilter = new Color(214, 53, 56);       //��ɫ

    float m_FireEffectFrequency = 3.0f;                    //��ɫת��Ƶ��
    float m_Timer = 0f;     //������ɫת��




    PostProcessVolume m_PostProcessVolume;
    ColorGrading m_ColorGrading;                //��ɫ�������
    Vignette m_Vignette;                        //��Ļ�۽���أ�����ģ���ֵ�Ͳ��ֻ����ҿ�����ΧһС�������







    private void Awake()
    {
        m_PostProcessVolume = GetComponent<PostProcessVolume>();

        
        if (m_PostProcessVolume != null )
        {
            //���Ի�ȡ�����������һ��û�л�ȡ���Ļ��򱨴�
            if (!m_PostProcessVolume.profile.TryGetSettings(out m_ColorGrading) || !m_PostProcessVolume.profile.TryGetSettings(out m_Vignette))
            {
                Debug.LogError("Some components are not assigned in the PostProcess volume.");
            }
        }

        else
        {
            Debug.LogError("The PostProcess volume component is missing.");
        }
    }

    private void Update()
    {
        //FireEffect();
    }






    #region ColorGrading���
    //����������
    public void AdjustBrightness(float newBrightness, float duration)
    {
        if (m_ColorGrading != null)
        {
            //�ȼ��Ҫ������ֵ�Ƿ���ڵ�ǰ��ֵ
            if (m_ColorGrading.postExposure.value != newBrightness)
            {
                //��һ��ʱ�䣨�ڶ���������֮�ڽ������Ӱֵ�ӵ�ǰ��ֵ��Ϊһ��ֵ����һ����������ʵ�ֱ䰵/������Ч��   
                DOTween.To(() => m_ColorGrading.postExposure.value, x => m_ColorGrading.postExposure.value = x, newBrightness, duration);
            }   
        }

        else
        {
            Debug.LogError("The Color grading component component is missing.");
        }
    }

    //�ȱ䰵�������������ڱ�ʾһ˲�����˸��
    public void DarkenThenBrighten(float newBrightness, float duration)
    {
        if (m_ColorGrading != null)
        {
            //�ȼ��Ҫ������ֵ�Ƿ���ڵ�ǰ��ֵ
            if (m_ColorGrading.postExposure.value != newBrightness)
            {
                //��֮ǰ�ȱ��浱ǰ������ֵ
                float currentValue = m_ColorGrading.postExposure;

                //�������Ӱֵ�ӵ�ǰ��ֵ��Ϊһ����һ��ֵ���������
                DOTween.To(() => m_ColorGrading.postExposure.value, x => m_ColorGrading.postExposure.value = x, newBrightness, duration);
                DOTween.To(() => m_ColorGrading.postExposure.value, x => m_ColorGrading.postExposure.value = x, currentValue, duration);
            }
        }

        else
        {
            Debug.LogError("The Color grading component component is missing.");
        }
    }
    #endregion


    #region Vignette���
    private void FireEffect()       //ͨ��������ɫ�˾���ģ����ұ������Χ
    {
        if (m_Vignette != null)
        {
            //���¼�ʱ��
            m_Timer += Time.deltaTime * m_FireEffectFrequency;

            //����Ƶ���ں�ɫ�ͳ�ɫ֮��ת��
            float t = Mathf.Sin(m_Timer) * 0.5f + 0.5f;
            Color currentColor = Color.Lerp(OrangeFilter, RedFilter, t);

            //��ֵ�µ���ɫ
            m_Vignette.color.Override(currentColor);
        }
    }
    #endregion
}
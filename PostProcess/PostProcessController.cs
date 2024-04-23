using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public class PostProcessController : MonoBehaviour
{
    PostProcessVolume m_PostProcessVolume;
    ColorGrading m_ColorGrading;






    private void Awake()
    {
        m_PostProcessVolume = GetComponent<PostProcessVolume>();

        
        if (m_PostProcessVolume != null )
        {
            //���Ի�ȡ������ɫ�������û�л�ȡ���Ļ��򱨴�
            if (!m_PostProcessVolume.profile.TryGetSettings(out m_ColorGrading))
            {
                Debug.LogError("The PostProcess volume doesn't has a color grading component.");
            }
        }

        else
        {
            Debug.LogError("The PostProcess volume component is missing.");
        }
    }



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
}
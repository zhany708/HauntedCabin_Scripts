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
            //尝试获取调整颜色的组件，没有获取到的话则报错
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
            //先检查要调整的值是否等于当前的值
            if (m_ColorGrading.postExposure.value != newBrightness)
            {
                //在一定时间（第二个参数）之内将相机阴影值从当前的值变为一个值（第一个参数），实现变暗/变亮的效果   
                DOTween.To(() => m_ColorGrading.postExposure.value, x => m_ColorGrading.postExposure.value = x, newBrightness, duration);
            }   
        }

        else
        {
            Debug.LogError("The Color grading component component is missing.");
        }
    }

    //先变暗，随后变亮（用于表示一瞬间的闪烁）
    public void DarkenThenBrighten(float newBrightness, float duration)
    {
        if (m_ColorGrading != null)
        {
            //先检查要调整的值是否等于当前的值
            if (m_ColorGrading.postExposure.value != newBrightness)
            {
                //改之前先保存当前的明暗值
                float currentValue = m_ColorGrading.postExposure;

                //将相机阴影值从当前的值变为一个另一个值，随后变回来
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
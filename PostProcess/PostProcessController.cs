using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public class PostProcessController : MonoBehaviour
{
    public static PostProcessController Instance { get; private set; }  //目前只有一个后期处理，所以可以用单例模式


    //更改颜色滤镜相关   
    public Color OrangeFilter = new Color(250, 107, 58);   //橙色
    public Color RedFilter = new Color(214, 53, 56);       //红色

    float m_FireEffectFrequency = 3.0f;                    //颜色转变频率
    float m_Timer = 0f;     //用于颜色转变




    PostProcessVolume m_PostProcessVolume;      //当前后期处理的Volume（可以有多个）
    ColorGrading m_ColorGrading;                //颜色处理相关
    Vignette m_Vignette;                        //屏幕聚焦相关（比如模拟手电筒，只让玩家看到周围一小块面积）







    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }


        m_PostProcessVolume = GetComponent<PostProcessVolume>();    //先获取Volume，随后再获取Volume内的组件
    
        if (m_PostProcessVolume != null )
        {
            //尝试获取所有组件，有一个没有获取到的话则报错
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


    #region ColorGrading相关
    //调整明亮度
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
            Debug.LogError("The Color grading component is missing.");
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
    #endregion


    #region Vignette相关
    public void FireEffect()       //通过调整颜色滤镜来模拟玩家被火焰包围
    {
        if (m_Vignette != null)
        {
            m_Vignette.enabled.value = true;    //打开Vignette

            //根据当前时间更新闪烁频率
            m_Timer += Time.deltaTime * m_FireEffectFrequency;

            //根据频率在红色和橙色之间转换
            float t = Mathf.Sin(m_Timer) * 0.5f + 0.5f;
            Color currentColor = Color.Lerp(OrangeFilter, RedFilter, t);

            //赋值新的颜色
            m_Vignette.color.Override(currentColor);
        }
    }

    public IEnumerator StartFireEffect()
    {
        while(true)
        {
            FireEffect();

            yield return new WaitForFixedUpdate();      //保证火焰滤镜的频率不变
        }       
    }


    public void TurnOffVignette()
    {
        m_Vignette.enabled.value = false;    //关闭Vignette
    }
    #endregion


    #region 其余函数
    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //每当进入一楼场景时都调用以下逻辑
        if (scene.name == "FirstFloor")
        {
            
        }

        //进入其余场景时（目前只有主菜单）
        else
        {
            //重置游戏
            ResetGame()
        }
    }


    public void ResetGame()     //重置游戏
    {
        TurnOffVignette();
    }
    #endregion
}
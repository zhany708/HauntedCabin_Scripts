using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;



public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager Instance { get; private set; }  //目前只有一个后期处理，所以可以用单例模式


    //更改颜色滤镜相关   
    [SerializeField] Color m_OrangeFilter = new Color(250, 107, 58);    //橙色
    [SerializeField] Color m_RedFilter = new Color(214, 53, 56);        //红色

    float m_FireEffectFrequency = 3.0f;                     //颜色转变频率
    float m_FireEffectTimer = 0f;                           //用于颜色转变



    PostProcessVolume m_PostProcessVolume;      //当前后期处理的Volume（可以有多个）
    ColorGrading m_ColorGrading;                //颜色处理相关
    Vignette m_Vignette;                        //屏幕聚焦相关（比如模拟手电筒，只让玩家看到周围一小块面积）


    Sequence DarkenSequence;                    //用于玩家离开房间后一瞬间变暗并恢复的Sequence





    #region Unity内部函数
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
                return;
            }
        }

        else
        {
            Debug.LogError("The PostProcess volume component is missing in the: " + gameObject.name);
            return;
        }
    }

    private void OnDestroy()
    {
        DarkenSequence.Kill();      //物体摧毁时杀死Sequence
    }
    #endregion


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
            Debug.LogError("The Color grading component is missing in the: " + gameObject.name);
            return;
        }
    }

    //先变暗，随后变亮（用于表示一瞬间的闪烁）
    public void DarkenThenBrighten(float newBrightness, float duration)
    {
        if (m_ColorGrading != null)
        {
            //进行更改之前先保存当前的明暗值
            float currentValue = m_ColorGrading.postExposure.value;


            //将相机阴影值从当前的值变为一个另一个值，随后变回来（使用DOTween的Sequence从而进行连续的多个DOTween）
            DarkenSequence = DOTween.Sequence();
            DarkenSequence.Append(DOTween.To(() => m_ColorGrading.postExposure.value, x => m_ColorGrading.postExposure.value = x, newBrightness, duration) );
            DarkenSequence.Append(DOTween.To(() => m_ColorGrading.postExposure.value, x => m_ColorGrading.postExposure.value = x, currentValue, duration) );            
        }

        else
        {
            Debug.LogError("The Color grading component is missing in the: " + gameObject.name);
            return;
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
            m_FireEffectTimer += Time.deltaTime * m_FireEffectFrequency;

            //根据频率在红色和橙色之间转换
            float t = Mathf.Sin(m_FireEffectTimer) * 0.5f + 0.5f;
            Color currentColor = Color.Lerp(m_OrangeFilter, m_RedFilter, t);

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
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            //重置游戏
            ResetGame();
        }
    }


    public void ResetGame()     //重置游戏
    {
        TurnOffVignette();
    }
    #endregion
}
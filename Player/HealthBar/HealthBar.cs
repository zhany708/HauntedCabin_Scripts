using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    Image m_HpImage;
    Image m_HpEffectImage;     //血量缓冲图片

    Player m_Player;


    float m_MaxHp;
    float m_CurrentHp;
    float m_BuffTime = 0.5f;    //缓冲时间

    Coroutine m_UpdateCoroutine;    //防止上一轮协程还没结束就开始新的协程（扣血）




    private IEnumerator Start()     //因为需要异步加载UI。所以使用协程而不是void
    {
        //等待UI加载完毕
        yield return UIManager.Instance.OpenPanel(UIConst.PlayerStatusBar);    //显示玩家状态栏

        //UI加载完毕后才会获取组件
        m_Player = GetComponentInParent<Player>();

        m_MaxHp = m_Player.PlayerData.MaxHealth;        //游戏开始时初始化最大生命值
        m_CurrentHp = m_MaxHp;

        //这个时候再运行此函数。就不会出现某个异步加载的组件还没初始化完毕就需要使用
        UpdateHealthBar();
    }




    public void SetCurrentHealth(float health)
    {
        m_CurrentHp = Mathf.Clamp(health, 0f, m_MaxHp);     //返回的值限制在0和血量上限之间
        UpdateHealthBar();
    }


    private void UpdateHealthBar()
    {
        //使用组件前检查是否为空
        if (m_HpImage == null || m_HpEffectImage == null)
        {
            Debug.LogError("Health bar images are not set.");
            return;
        }


        m_HpImage.fillAmount = m_CurrentHp / m_MaxHp;

        if (m_UpdateCoroutine != null)      
        {
            StopCoroutine(m_UpdateCoroutine);     //如果协程正在进行，则停止它然后开始新的协程，保证只有一个协程存在
        }

        m_UpdateCoroutine = StartCoroutine(UpdateHpEffect());
    }





    private IEnumerator UpdateHpEffect()
    {
        float effectLength = m_HpEffectImage.fillAmount - m_HpImage.fillAmount;     //缓冲的血量
        float elapsedTime = 0f;     //用于确保缓冲时间在0.5秒内

        while (elapsedTime < m_BuffTime && effectLength != 0f)
        {
            elapsedTime += Time.deltaTime;
            m_HpEffectImage.fillAmount = Mathf.Lerp(m_HpImage.fillAmount + effectLength, m_HpImage.fillAmount, elapsedTime/m_BuffTime);   //返回值根据第三个参数决定， 0则返回参数一，1则返回参数二，0.5则返回中点

            yield return null;      //等待一帧的时间
        }

        m_HpEffectImage.fillAmount = m_HpImage.fillAmount;      //防止缓冲血条超过红色血条
    }


    #region Setters
    public void SetHpImage(Image thisImage)
    {
        m_HpImage = thisImage;
    }

    public void SetHpEffectImage(Image thisImage)
    {
        m_HpEffectImage = thisImage;
    }
    #endregion
}

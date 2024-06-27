using System.Collections;
using UnityEngine;
using UnityEngine.UI;



//所有血条的跟类（用于控制血条UI，但是自身不是UI）
public class HealthBar : MonoBehaviour
{
    protected Image hpImage { get; private set; }                   //最上层的血条图片（红色的）
    protected Image decreaseHpEffectImage { get; private set; }     //扣血缓冲图片（白色的）
    protected Image increaseHpEffectImage { get; private set; }     //回血的缓冲图片（绿色的）



    Coroutine m_UpdateHealthCoroutine;      //用于血条整体变化的协程

    float m_MaxHp = 0f;                     //最大血量默认0（需要在不同的子类中设置）
    float m_CurrentHp = 0f;
    float m_BuffTime = 0.5f;                //缓冲时间

    







    protected virtual void Start()     
    {
        InitializeHealthBar();
    }
    


    //因为需要异步加载UI。所以使用async（如果不使用的话，可能会出现还没加载完就接着跑下面的代码的情况）
    private void InitializeHealthBar()   
    {       
        //游戏开始时检查是否设置了最大生命值，随后将当前生命值设置为最大生命值
        if (m_MaxHp != 0f)
        {
            m_CurrentHp = m_MaxHp;
        }
        else
        {
            Debug.Log("You haven't set the maxHp in the " + name);
        }
        
        //注意运行顺序，防止出现某个异步加载的组件还没初始化完毕就需要使用的情况
        UpdateHealthBar();
    }


    #region 管理血条相关
    public void SetCurrentHealth(float health)
    {
        m_CurrentHp = Mathf.Clamp(health, 0f, m_MaxHp);     //将当前血量限制在0和血量上限之间
        UpdateHealthBar();
    }


    protected void UpdateHealthBar()
    {
        //使用组件前检查是否为空
        if (hpImage == null || increaseHpEffectImage == null || decreaseHpEffectImage == null)
        {
            Debug.LogError("Some health bar images are not set.");
            return;
        }



        if (m_UpdateHealthCoroutine != null)
        {
            StopCoroutine(m_UpdateHealthCoroutine);
        }

        m_UpdateHealthCoroutine = StartCoroutine(UpdateHealthBarSequence() ); 
    }

    private IEnumerator UpdateHealthBarSequence()       //用于血条整体变化的函数
    {
        //永远先进行回血缓冲，随后更改血条，最后进行扣血缓冲（三者不可同时执行）
        yield return StartCoroutine(IncreaseHpEffect());

        //调整红色血条的比例
        hpImage.fillAmount = m_CurrentHp / m_MaxHp;

        //等上述内容都运行完后，才执行扣血缓冲
        yield return StartCoroutine(DecreaseHpEffect());
    }


    //用于回血缓冲的动画（此时血条占比还没有变）
    private IEnumerator IncreaseHpEffect()
    {
        //由于此时红色血条的比例还没变，所以用一个临时变量表示血条的占比
        float futureHpImageFillAmount = m_CurrentHp / m_MaxHp;        

        float effectLength = increaseHpEffectImage.fillAmount - futureHpImageFillAmount;     //缓冲的血量（回血缓冲血条的比例减去血条比例）
        float elapsedTime = 0f;     //用于确保缓冲时间在0.5秒内


        //此while循环用于将缓冲血条比例在缓冲时间内从一个值变到另一个值
        while (elapsedTime < m_BuffTime && effectLength != 0f)
        {
            //持续增加变量值，直到抵达预定的缓冲时间为止
            elapsedTime += Time.deltaTime;

            //血条从高往低降或从低往高升：返回值根据第三个参数决定， 0则返回参数一，1则返回参数二，0.5则返回中点
            increaseHpEffectImage.fillAmount = Mathf.Lerp(futureHpImageFillAmount + effectLength, futureHpImageFillAmount, elapsedTime / m_BuffTime);             
                
            yield return null;      //等待一帧的时间
        }

        increaseHpEffectImage.fillAmount = futureHpImageFillAmount;      //防止回血缓冲跟红色血条占比不一致       
    }

    //用于扣血缓冲血条的动画（此时血条占比已经变化过了）
    private IEnumerator DecreaseHpEffect()
    {
        float effectLength = decreaseHpEffectImage.fillAmount - hpImage.fillAmount;     //缓冲的血量（扣血缓冲血条的比例减去血条比例）
        float elapsedTime = 0f;


        while (elapsedTime < m_BuffTime && effectLength != 0f)
        {
            elapsedTime += Time.deltaTime;

            decreaseHpEffectImage.fillAmount = Mathf.Lerp(hpImage.fillAmount + effectLength, hpImage.fillAmount, elapsedTime / m_BuffTime);             
                
            yield return null;
        }

        decreaseHpEffectImage.fillAmount = hpImage.fillAmount;              //防止扣血缓冲血条跟红色血条占比不一致        
    }
    #endregion


    #region Setters
    public void SetMaxHp(float thisMaxHp)
    {
        m_MaxHp = thisMaxHp;
    }

    public void SetHpImage(Image thisImage)
    {
        hpImage = thisImage;
    }

    public void SetIncreaseHpEffectImage(Image thisImage)
    {
        increaseHpEffectImage = thisImage;
    }

    public void SetDecreaseHpEffectImage(Image thisImage)
    {
        decreaseHpEffectImage = thisImage;
    }
    #endregion
}
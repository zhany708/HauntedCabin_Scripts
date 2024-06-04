using UnityEngine;



public class ParticleController : MonoBehaviour
{
    private void DestroyGameObject()    //用于动画帧事件
    {
        ParticlePool.Instance.PushObject(gameObject);
    }
}
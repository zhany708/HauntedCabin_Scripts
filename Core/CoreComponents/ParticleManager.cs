using Unity.Mathematics;
using UnityEngine;

public class ParticleManager : ManagerTemplate<ParticleManager>     //用于管理特效
{



    //基础生成特效函数
    public GameObject StartParticles(GameObject particlePrefab, Vector2 position, quaternion rotation)
    {
        GameObject particle = ParticlePool.Instance.GetObject(particlePrefab);
        particle.transform.position = position;
        particle.transform.rotation = rotation;

        return particle;
    }

    //生成特效于物体中心
    public GameObject StartParticles(GameObject particlePrefab)
    {
        return StartParticles(particlePrefab, transform.position, quaternion.identity);      
    }

    //生成随机角度的特效
    public GameObject StartParticleWithRandomRotation(GameObject particlePrefab)
    {
        var randomRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));      //生成随机角度

        return StartParticles(particlePrefab, transform.position, randomRotation);
    }
}
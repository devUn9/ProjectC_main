using UnityEngine;
using Unity.Cinemachine;

public class GrenadeEffect : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void OnEnable()
    {
        TriggerShake();
    }

    public void TriggerShake()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}

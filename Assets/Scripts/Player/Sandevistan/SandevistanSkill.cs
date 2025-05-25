using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SandevistanSkill : Skill
{
    [SerializeField] private Volume myVolume; // Inspector에서 Volume 컴포넌트 할당
    [SerializeField] private float duration = 3f; // 산데비스탄 포스트프로세싱 시간

    [Header("Sandevistan Info")]
    public float sandevistanDuration = 3f; // 지속 시간

    private void Awake()
    {
    }
    public IEnumerator TimeScaleModify()
    {
        SandevistanPost.Instance.ActivateVolumeForDuration(myVolume, duration);
        TimeManager.Instance.timeScale = 0.15f;
        Debug.Log("TimeScale : " + TimeManager.Instance.timeScale);
        yield return new WaitForSeconds(sandevistanDuration);
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    ReturnTimeScale();
        //}
        TimeManager.Instance.timeScale = 1f;
        Debug.Log("TimeScale : " + TimeManager.Instance.timeScale);
    }

    public void ReturnTimeScale()
    {
        TimeManager.Instance.timeScale = 1f;
    }

    public virtual bool SandevistanCanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            cooldownTimer = cooldown + sandevistanDuration;
            return true;
        }
        return false;
    }
}

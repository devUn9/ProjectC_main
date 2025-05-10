using System.Collections;
using UnityEngine;

public class SandevistanSkill : Skill
{
    [Header("Sandevistan Info")]
    public float sandevistanDuration = 5f; // 지속 시간
    
    private void Awake()
    {
    }
    public IEnumerator TimeScaleModify()
    {
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
}

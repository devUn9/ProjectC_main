using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class SandevistanPost : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SandevistanPost Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 설정: 인스턴스가 없으면 현재 오브젝트를 사용, 중복 시 기존 오브젝트 파괴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Volume을 지정된 시간 동안 활성화하는 공용 메서드
    public void ActivateVolumeForDuration(Volume volume, float duration)
    {
        if (volume == null)
        {
            Debug.LogError("Provided Volume component is null!");
            return;
        }

        StartCoroutine(ActivateVolumeCoroutine(volume, duration));
    }

    // Volume을 지정된 시간 동안 활성화하는 코루틴
    private IEnumerator ActivateVolumeCoroutine(Volume volume, float duration)
    {
        // Volume 오브젝트 활성화
        volume.gameObject.SetActive(true);
        Debug.Log($"Volume activated for {duration} seconds.");

        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(duration);

        // Volume 오브젝트 비활성화
        volume.gameObject.SetActive(false);
        Debug.Log("Volume deactivated.");
    }
}
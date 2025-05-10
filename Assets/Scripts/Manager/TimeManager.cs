using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance; // 싱글톤 인스턴스
    public static TimeManager Instance { get { return instance; } }

    public float timeScale = 1.0f; // 기본 시간 비율


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
        }
    }
}

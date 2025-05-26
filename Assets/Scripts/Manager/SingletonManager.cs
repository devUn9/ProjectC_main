using Unity.VisualScripting;
using UnityEngine;

public class SingletonManager : MonoBehaviour
{

    public static SingletonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지되게 하는 함수
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

}

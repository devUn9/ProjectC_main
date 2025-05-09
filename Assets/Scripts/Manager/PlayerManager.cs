using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // 플레이어 참조용으로 만든 임시 스크립트입니다. 수정 필요하시면 마음대로 수정하셔도 됩니다.

    public static PlayerManager instance { get; private set; }
    public GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}

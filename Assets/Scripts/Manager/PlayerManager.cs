using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // �÷��̾� ���������� ���� �ӽ� ��ũ��Ʈ�Դϴ�. ���� �ʿ��Ͻø� ������� �����ϼŵ� �˴ϴ�.

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

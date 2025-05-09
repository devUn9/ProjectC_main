using UnityEngine;

public class BedMove : MonoBehaviour
{
    [SerializeField] private Transform bedPos; // 침대 이동 위치
    [SerializeField] private string playerTag = "Player"; // 플레이어 태그
    [SerializeField] private float inputCooldown = 0.5f; // Spacebar 입력 쿨다운

    private float lastInputTime = 0f; // 마지막 입력 시간

    private void Awake()
    {
        // BoxCollider2D 확인
        if (!GetComponent<BoxCollider2D>())
        {
            Debug.LogError("BedMove에 BoxCollider2D가 없습니다!", this);
        }

        // bedPos가 지정되지 않은 경우, 자체 Transform 사용
        if (bedPos == null)
        {
            bedPos = transform;
            Debug.LogWarning("bedPos가 지정되지 않았습니다. 자체 Transform을 사용합니다.", this);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어와 충돌 중이고 Spacebar를 눌렀는지 확인
        if (collision.gameObject.CompareTag(playerTag) && Input.GetKeyDown(KeyCode.Space) && Time.unscaledTime - lastInputTime >= inputCooldown)
        {
            lastInputTime = Time.unscaledTime;

            // 플레이어를 침대 위치로 이동
            collision.gameObject.transform.position = bedPos.position;
            Debug.Log("플레이어가 침대 위치로 이동했습니다.");
        }
    }

    private void OnValidate()
    {
        // Inspector에서 필수 참조가 누락되었는지 확인
        if (bedPos == null)
            Debug.LogWarning("BedMove에 bedPos가 지정되지 않았습니다.", this);
    }
}
using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    // 컴포넌트 참조
    private Rigidbody2D rb;
    private GrappleHook gh;

    [Header("이동 설정")]
    [SerializeField] private float speed = 5f;   // 플레이어 이동 속도

    private float mx;  // 수평 입력
    private float my;  // 수직 입력

    private void Start()
    {
        // Rigidbody2D와 GrappleHook 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
        gh = GetComponent<GrappleHook>();

        // 컴포넌트 누락 시 경고
        if (gh == null)
            Debug.LogWarning("GrappleHook 컴포넌트가 이 오브젝트에 없습니다!");
    }

    private void Update()
    {
        // 입력값 받기 (즉각 반응형)
        mx = Input.GetAxisRaw("Horizontal");
        my = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        // 그래플링 상태가 아닐 때만 이동 가능
        if (!gh.retracting)
        {
            rb.linearVelocity = new Vector2(mx, my).normalized * speed;
        }
        else
        {
            // 그래플링 중일 때 이동 멈춤
            rb.linearVelocity = Vector2.zero;
        }
    }
}

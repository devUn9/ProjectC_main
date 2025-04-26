using System.Collections;
using UnityEngine;

public class WallGrapple : MonoBehaviour
{
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask wallMask;       // 벽 그래플링 전용 레이어
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;

    private bool isGrappling = false;
    private Vector2 target;
    private bool isRetractingPlayer = false;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        if (isRetractingPlayer)
            HandlePlayerRetract();
    }

    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, wallMask);

        if (hit.collider != null)
        {
            Debug.Log("벽 그래플링 시작: " + hit.collider.name);

            isGrappling = true;
            line.enabled = true;
            line.positionCount = 2;

            target = hit.point;
            StartCoroutine(Grapple(target));
        }
        else
        {
            Debug.Log("벽 그래플링 실패");
        }
    }

    private void HandlePlayerRetract()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, grappleSpeed * Time.deltaTime);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            ResetGrapple();
        }
    }

    private void ResetGrapple()
    {
        isRetractingPlayer = false;
        isGrappling = false;
        line.enabled = false;
    }

    IEnumerator Grapple(Vector2 targetPosition)
    {
        float t = 0f;
        float time = 0.2f;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        while (t < time)
        {
            t += Time.deltaTime;
            Vector2 newPos = Vector2.Lerp(transform.position, targetPosition, t / time);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            yield return null;
        }

        line.SetPosition(1, targetPosition);
        isRetractingPlayer = true;
    }
}

using System.Collections;
using UnityEngine;

public class GrappleHook5 : MonoBehaviour
{
    private LineRenderer line;

    [Header("Grappling ¼³Á¤")]
    [SerializeField] LayerMask grapplableMask;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;

    private bool isGrappling = false;

    private Vector2 target;
    private Transform targetObject;
    private float retractTimer = 0f;
    public int itemCount;
    private Vector2 pullStopPosition;

    [HideInInspector] public bool isRetractingPlayer = false;
    [HideInInspector] public bool isRetractingObject = false;

    public bool isUpgrade = false;
    public float SpeedMultiplier => isUpgrade ? 1.5f : 1f;

    private bool isTargetLocked = false;
    private RaycastHit2D lockedHit;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isGrappling && !isTargetLocked)
                LockTarget();
            else if (isTargetLocked && !isGrappling)
                ExecuteGrapple();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
            CancelGrappleTarget();

        if (line.enabled)
            line.SetPosition(0, transform.position);

        if (isTargetLocked)
        {
            float distanceToTarget = Vector2.Distance(transform.position, lockedHit.point);
            if (distanceToTarget > maxDistance)
                CancelGrappleTarget();
        }

        if (isRetractingPlayer)
            HandlePlayerRetract();

        if (isRetractingObject)
            HandleObjectRetract();

        if (isUpgrade)
            SpeedUpgrade();
    }

    private void SpeedUpgrade() { }

    private void StartGrapple()
    {
        Vector2 origin = transform.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isGrappling = true;
            line.enabled = true;
            line.positionCount = 2;

            int targetLayer = hit.collider.gameObject.layer;

            if (targetLayer == LayerMask.NameToLayer("Grappleable"))
            {
                targetObject = hit.collider.transform;

                Collider2D playerCol = GetComponentInParent<Collider2D>();
                Collider2D targetCol = targetObject.GetComponent<Collider2D>();

                if (playerCol && targetCol)
                    Physics2D.IgnoreCollision(playerCol, targetCol, true);

                Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
                pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;

                StartCoroutine(Grapple(targetObject.position, false));
            }
            else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
            {
                target = hit.point;
                StartCoroutine(Grapple(target, true));
            }
        }
    }

    private void HandlePlayerRetract()
    {
        transform.parent.position = Vector2.MoveTowards(transform.parent.position, target, grappleSpeed * Time.deltaTime);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        if (Vector2.Distance(transform.parent.position, target) < 0.5f)
            ResetGrapple();
    }

    private void HandleObjectRetract()
    {
        if (!targetObject)
        {
            ResetGrapple();
            return;
        }

        retractTimer += Time.deltaTime;
        targetObject.position = Vector2.MoveTowards(targetObject.position, pullStopPosition, grappleSpeed * Time.deltaTime);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetObject.position);

        if (Vector2.Distance(targetObject.position, pullStopPosition) < 0.1f || retractTimer > 3f)
        {
            if (targetObject.CompareTag("Collectible"))
            {
                itemCount++;
                Destroy(targetObject.gameObject);
            }
            else if (targetObject.CompareTag("Grapplable"))
            {
                StartCoroutine(StunObject(targetObject));
            }
            ResetGrapple();
        }
    }

    private void ResetGrapple()
    {
        if (targetObject)
        {
            Collider2D playerCol = GetComponentInParent<Collider2D>();
            Collider2D targetCol = targetObject.GetComponent<Collider2D>();

            if (playerCol && targetCol)
                Physics2D.IgnoreCollision(playerCol, targetCol, false);
        }

        isRetractingObject = false;
        isRetractingPlayer = false;
        isGrappling = false;
        line.enabled = false;
        targetObject = null;
        retractTimer = 0f;
    }

    IEnumerator Grapple(Vector2 targetPosition, bool isPlayerMoving)
    {
        float t = 0f;
        float time = 0.2f;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        while (t < time)
        {
            t += grappleShootSpeed * Time.deltaTime;
            Vector2 newPos = Vector2.Lerp(transform.position, targetPosition, t / time);
            line.SetPosition(1, newPos);
            yield return null;
        }

        line.SetPosition(1, targetPosition);

        if (isPlayerMoving)
            isRetractingPlayer = true;
        else
            isRetractingObject = true;
    }

    IEnumerator StunObject(Transform obj)
    {
        SpriteRenderer sp = obj.GetComponent<SpriteRenderer>();

        if (sp)
        {
            Color originalColor = sp.color;
            sp.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            sp.color = originalColor;
        }
        ResetGrapple();
    }

    private void LockTarget()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isTargetLocked = true;
            lockedHit = hit;

            line.enabled = true;
            line.positionCount = 2;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);
        }
    }

    private void ExecuteGrapple()
    {
        if (!lockedHit.collider)
        {
            isTargetLocked = false;
            line.enabled = false;
            return;
        }

        isGrappling = true;
        int targetLayer = lockedHit.collider.gameObject.layer;

        if (targetLayer == LayerMask.NameToLayer("Grappleable"))
        {
            targetObject = lockedHit.collider.transform;
            Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
            pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;
            StartCoroutine(Grapple(targetObject.position, false));
        }
        else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
        {
            target = lockedHit.point;
            StartCoroutine(Grapple(target, true));
        }

        isTargetLocked = false;
    }

    private void CancelGrappleTarget()
    {
        if (isTargetLocked)
        {
            isTargetLocked = false;
            line.enabled = false;
        }
    }
}
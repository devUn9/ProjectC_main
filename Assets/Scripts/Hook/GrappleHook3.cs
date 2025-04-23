using System;
using System.Collections;
using UnityEngine;

public class GrappleHook3 : MonoBehaviour
{
    // ���� �ð�ȭ�� ���� LineRenderer ������Ʈ
    private LineRenderer line;

    [Header("Grappling ����")]
    [SerializeField] LayerMask grapplableMask;   // �׷��ø� ������ ������Ʈ ���̾�
    [SerializeField] float maxDistance = 10f;     // �׷��ø� �ִ� ��Ÿ�
    [SerializeField] float grappleSpeed = 10f;    // ������Ʈ�� �������� �ӵ�
    [SerializeField] float grappleShootSpeed = 20f; // �� �߻� �ִϸ��̼� �ӵ�

    private bool isGrappling = false;             // ���� �׷��ø� ������ ����

    private Vector2 target;        // ���� ���� ��ġ (����� ������ ����)
    private Transform targetObject; // ������ ����� ��� ������Ʈ

    [HideInInspector] public bool isRetractingPlayer = false; // �÷��̾ ���������� ����
    [HideInInspector] public bool isRetractingObject = false; // ������Ʈ�� ���������� ����

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer ������Ʈ ����
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        if (isRetractingPlayer)
            HandlePlayerRetract();

        if (isRetractingObject)
            HandleObjectRetract();
    }

    // �׷��ø� �� �߻� �޼���
    private void StartGrapple()
    {

    }

    // �÷��̾ �������� ���� �߻�
    private void HandlePlayerRetract()
    {

    }

    // �÷��̾ ���� ���� ���� �߻�
    private void HandleObjectRetract()
    {
    }

}

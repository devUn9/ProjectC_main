using System;
using System.Collections;
using UnityEngine;

public class GrappleHook3 : MonoBehaviour
{
    // 로프 시각화를 위한 LineRenderer 컴포넌트
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask grapplableMask;   // 그래플링 가능한 오브젝트 레이어
    [SerializeField] float maxDistance = 10f;     // 그래플링 최대 사거리
    [SerializeField] float grappleSpeed = 10f;    // 오브젝트가 끌려오는 속도
    [SerializeField] float grappleShootSpeed = 20f; // 훅 발사 애니메이션 속도

    private bool isGrappling = false;             // 현재 그래플링 중인지 여부

    private Vector2 target;        // 훅이 맞은 위치 (현재는 사용되지 않음)
    private Transform targetObject; // 실제로 끌어올 대상 오브젝트

    [HideInInspector] public bool isRetractingPlayer = false; // 플레이어가 끌려가지는 여부
    [HideInInspector] public bool isRetractingObject = false; // 오브젝트가 끌려오는지 여부

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer 컴포넌트 참조
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

    // 그래플링 훅 발사 메서드
    private void StartGrapple()
    {

    }

    // 플레이어가 끌려가는 로프 발사
    private void HandlePlayerRetract()
    {

    }

    // 플레이어가 끌고 오는 로프 발사
    private void HandleObjectRetract()
    {
    }

}

using UnityEngine;
using Unity.Cinemachine;
public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera; // Cinemachine ���� ī�޶�
    [SerializeField] private float zoomSpeed = 5f; // �� �ӵ�
    [SerializeField] private float defaultSize = 5f; // �⺻ Orthographic Size
    [SerializeField] private float zoomedSize = 10f; // �� �ƿ��� Orthographic Size
    private float currentSize;
    private CinemachineConfiner2D confiner;
    private void Start()
    {
        // CinemachineConfiner2D ������Ʈ ��������
        confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner == null)
        {
            Debug.LogWarning("CinemachineConfiner2D ������Ʈ�� �����ϴ�. ��� �˻簡 ��Ȱ��ȭ�˴ϴ�.");
        }
        if (virtualCamera != null && virtualCamera.Lens.Orthographic)
        {
            currentSize = defaultSize;
            // �ʱ� ���� ����
            var lens = virtualCamera.Lens;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }
    private void Update()
    {
        if (virtualCamera != null && virtualCamera.Lens.Orthographic)
        {
            var lens = virtualCamera.Lens;
            float targetSize;
            // Ctrl Ű�� ���ȴ��� Ȯ��
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                // Ű�� ������ ������ �� �ƿ� �õ� (ũ�� ����)
                targetSize = zoomedSize;
            }
            else
            {
                // Ű�� ���� ���� ũ��� ����
                targetSize = defaultSize;
            }
            // ���� ũ��� ��ǥ ũ�� ������ ���� �ܰ� ���
            float nextSize = Mathf.Lerp(currentSize, targetSize, zoomSpeed * Time.deltaTime);
            // ���� -> ��ǥ�� ���� ������, ��ǥ -> ����� ���ƿ��� ������ Ȯ��
            bool isZoomingOut = nextSize > currentSize;
            // �� �ƿ��ϴ� ��쿡�� �ٿ�� üũ ����
            if (isZoomingOut && confiner != null && confiner.enabled)
            {
                // �� ũ��� ������Ʈ�ϱ� ���� �ٿ�� �˻�
                if (!WillStayWithinBounds(nextSize))
                {
                    // �浹�� �߻��ϸ� ũ�⸦ �������� ����
                    nextSize = currentSize;
                }
            }
            // ���� ũ�� ����
            currentSize = nextSize;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }
    private bool WillStayWithinBounds(float orthographicSize)
    {
        // Confiner�� ���ų� �ٿ�� �������� ������ true ��ȯ
        if (confiner == null || confiner.BoundingShape2D == null)
            return true;
        // Confiner���� �ٿ�� ������ ��������
        Collider2D boundingShape = confiner.BoundingShape2D;
        // ī�޶��� ȭ�� ũ�� ���
        float aspectRatio = (float)Screen.width / Screen.height;
        float cameraWidth = orthographicSize * 2f * aspectRatio;
        float cameraHeight = orthographicSize * 2f;
        // ī�޶� ȭ���� �� �𼭸��� ��� �ٿ�� ������ ���� �ִ��� Ȯ��
        Vector2 cameraPos = virtualCamera.transform.position;
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(cameraPos.x - cameraWidth / 2, cameraPos.y - cameraHeight / 2); // ���ϴ�
        corners[1] = new Vector2(cameraPos.x + cameraWidth / 2, cameraPos.y - cameraHeight / 2); // ���ϴ�
        corners[2] = new Vector2(cameraPos.x - cameraWidth / 2, cameraPos.y + cameraHeight / 2); // �»��
        corners[3] = new Vector2(cameraPos.x + cameraWidth / 2, cameraPos.y + cameraHeight / 2); // ����
        // ��� �𼭸��� �ٿ�� ������ ���� �ִ��� Ȯ��
        foreach (Vector2 corner in corners)
        {
            if (!boundingShape.OverlapPoint(corner))
            {
                return false; // �ϳ��� �ٿ�� ���̸� false ��ȯ
            }
        }
        return true; // ��� �ڳʰ� �ٿ�� �ȿ� ����
    }
}
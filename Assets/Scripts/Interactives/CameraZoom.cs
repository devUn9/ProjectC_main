using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera; // Cinemachine ���� ī�޶�
    [SerializeField] private float zoomSpeed = 5f; // �� �ӵ�
    [SerializeField] private float defaultSize = 5f; // �⺻ Orthographic Size
    [SerializeField] private float zoomedSize = 10f; // �� �ƿ��� Orthographic Size
    [SerializeField] private float walkSpeed = 2f; // Ctrl Ű ������ ���� �̵� �ӵ�
    [SerializeField] private Player player; // Player ��ũ��Ʈ ����

    private float currentSize;
    private float defaultPlayerSpeed; // ���� Player�� moveSpeed ����

    private void Start()
    {
        // Player ������Ʈ Ȯ��
        if (player == null)
        {
            player = GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Player ������Ʈ�� �������� �ʾҽ��ϴ�!");
            }
        }

        // ���� Player�� moveSpeed ����
        if (player != null)
        {
            defaultPlayerSpeed = player.moveSpeed;
        }

        // �ʱ� ���� ����
        if (virtualCamera != null && virtualCamera.Lens.Orthographic)
        {
            currentSize = defaultSize;
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
                if (player != null)
                {
                    player.moveSpeed = walkSpeed; // Player�� moveSpeed�� walkSpeed�� ����
                }
            }
            else
            {
                // Ű�� ���� ���� ũ��� �ӵ��� ����
                targetSize = defaultSize;
                if (player != null)
                {
                    player.moveSpeed = defaultPlayerSpeed; // ���� moveSpeed�� ����
                }
            }

            // ���� ũ��� ��ǥ ũ�� ������ ���� �ܰ� ���
            float nextSize = Mathf.Lerp(currentSize, targetSize, zoomSpeed * Time.deltaTime);

            // ���� ũ�� ����
            currentSize = nextSize;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }
}
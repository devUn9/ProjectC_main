using UnityEngine;
using Unity.Cinemachine;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameObject player; // �÷��̾� ������Ʈ
    [SerializeField] private Transform targetPosition; // �̵��� ��ǥ ��ǥ
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine ���� ī�޶�
    [SerializeField] private BoxCollider2D targetBoundingShape; // ��ǥ ��ġ�� Bounding Shape 2D
    

    private CinemachineConfiner2D confiner; // Cinemachine Confiner 2D ������Ʈ

    private void Awake()
    {
        // Cinemachine Confiner 2D ������Ʈ ��������
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
            {
                Debug.LogError("CinemachineConfiner2D ������Ʈ�� ���� ī�޶� �����ϴ�!");
            }
            // Follow �ʱ�ȭ
            virtualCamera.Follow = player.transform; // ���⼭ Follow ����
        }
        else
        {
            Debug.LogError("Virtual Camera�� �������� �ʾҽ��ϴ�!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // �浹�� ������Ʈ�� �÷��̾����� Ȯ��
        if (collision.gameObject == player)
        {
            // �÷��̾� ��ġ �̵�
            player.transform.position = new Vector3(targetPosition.position.x, targetPosition.position.y + 1, player.transform.position.z);

            // ī�޶� ��� ������Ʈ
            if (confiner != null && targetBoundingShape != null)
            {
                confiner.BoundingShape2D = targetBoundingShape; // ���ο� ��� ����
                confiner.InvalidateBoundingShapeCache(); // ĳ�� ��ȿȭ�� ��� ����
                virtualCamera.ForceCameraPosition(player.transform.position, Quaternion.identity); 
            }
            else
            {
                Debug.LogWarning("Confiner �Ǵ� Target Bounding Shape�� �������� �ʾҽ��ϴ�!");
            }
        }
    }
}
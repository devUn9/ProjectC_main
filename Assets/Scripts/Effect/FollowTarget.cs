using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public bool followPosition = true;
    public bool followRotation = true;
    public Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        if (followPosition)
            transform.position = target.position + offset;

        if (followRotation)
            transform.rotation = target.rotation;
    }
}
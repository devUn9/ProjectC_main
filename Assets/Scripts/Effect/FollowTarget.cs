using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

      
        if (target != null)
        {
            transform.position = target.TransformPoint(offset);
            transform.rotation = target.rotation;
        }
        
    }
}
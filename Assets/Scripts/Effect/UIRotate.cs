using UnityEngine;

public class UIRotate : MonoBehaviour
{
    public float rotationSpeed = 90f; 

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}

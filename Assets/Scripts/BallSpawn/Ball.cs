using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] public GameObject target;
    [SerializeField] float speed;
    Vector3 dir;
    Vector3 target2 = new Vector3 (50, 300, 0);
    void Start()
    {
        dir = (target2 - transform.position).normalized;
    }

    void Update()
    {
        transform.Translate (dir * speed * Time.deltaTime * TimeManager.Instance.timeScale);
    }
}

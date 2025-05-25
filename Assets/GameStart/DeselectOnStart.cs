using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeselectOnStart : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DeselectNextFrame());
    }

    private IEnumerator DeselectNextFrame()
    {
        yield return null; // 1 프레임 기다려야 Unity 내부 초기화 완료됨
        EventSystem.current.SetSelectedGameObject(null);
    }
}

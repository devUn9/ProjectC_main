using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private Material GravitonCharge;
    private Material originalMat;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        yield return new WaitForSeconds(0.2f);
        sr.material = originalMat;
    }

    private IEnumerator ChargeFX()
    {
        sr.material = GravitonCharge;
        yield return new WaitForSeconds(0.1f);
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelRedBlink()
    {
        CancelInvoke();
        sr.color = Color.white;
    }
}

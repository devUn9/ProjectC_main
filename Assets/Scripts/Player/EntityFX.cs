using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private Material GravitonCharge;
    [SerializeField] private Material Stun;
    [SerializeField] private Material EmpShock1;
    [SerializeField] private Material EmpShock2;
    private Material originalMat;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    private IEnumerator StunFX()
    {
        sr.material = Stun;
        yield return new WaitForSeconds(0.1f);
        sr.material = originalMat;
    }

    public IEnumerator EmpShockFX(float _duration)
    {
        while (_duration > 0)
        {
            sr.material = EmpShock1;
            yield return new WaitForSeconds(0.1f);
            sr.material = EmpShock2;
            yield return new WaitForSeconds(0.1f);
            _duration -= Time.deltaTime * TimeManager.Instance.timeScale;
        }
        sr.material = originalMat;
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

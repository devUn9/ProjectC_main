using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class ShadowCasterFixer : MonoBehaviour
{
    [MenuItem("Tools/Fix All Shadow Casters (Keep All Settings)")]
    static void FixShadowCasters()
    {
        var allCasters = GameObject.FindObjectsOfType<ShadowCaster2D>(true);
        int fixedCount = 0;

        foreach (var caster in allCasters)
        {
            GameObject obj = caster.gameObject;

            // 기존 설정 백업
            bool wasEnabled = caster.enabled;
            bool castsShadows = caster.castsShadows;
            bool selfShadows = caster.selfShadows;
            bool useRendererSilhouette = caster.useRendererSilhouette;

            // shapePath도 백업 (Vector3[])
            Vector3[] shapePath = new Vector3[caster.shapePath.Length];
            caster.shapePath.CopyTo(shapePath, 0);

            // 기존 ShadowCaster 제거
            DestroyImmediate(caster);

            // 새로 추가
            var newCaster = obj.AddComponent<ShadowCaster2D>();

            // 설정 복원
            newCaster.enabled = wasEnabled;
            newCaster.castsShadows = castsShadows;
            newCaster.selfShadows = selfShadows;
            newCaster.useRendererSilhouette = useRendererSilhouette;

            fixedCount++;
        }

        Debug.Log($"Fixed {fixedCount} ShadowCaster2D components and restored all settings.");
    }
}

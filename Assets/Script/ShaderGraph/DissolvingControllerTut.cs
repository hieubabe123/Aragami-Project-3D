using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingControllerTut : MonoBehaviour
{
    private const string DISSOLVE_AMOUNT_PROPERTY = "_DissolveAmount";
    [Header("------------------------Dissolving Settings------------------------")]
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect vfxDissolve;

    private float _dissolveDuration = 0.6f;
    private List<Material> allMaterials = new List<Material>();
    void Awake()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            Material[] currentMaterials = renderer.materials;

            for (int i = 0; i < currentMaterials.Length; i++)
            {
                Material materialInstance = new Material(currentMaterials[i]);
                allMaterials.Add(materialInstance);
                currentMaterials[i] = materialInstance;
            }

            renderer.materials = currentMaterials;
        }

        foreach (var renderer in meshRenderers)
        {
            Material[] currentMaterials = renderer.materials;

            for (int i = 0; i < currentMaterials.Length; i++)
            {
                Material materialInstance = new Material(currentMaterials[i]);
                allMaterials.Add(materialInstance);
                currentMaterials[i] = materialInstance;
            }

            renderer.materials = currentMaterials;
        }
    }

    public IEnumerator DissolveCo(float dissolveDuration)
    {
        if (vfxDissolve != null) { }
        {
            vfxDissolve.Play();
        }
        if (allMaterials.Count > 0)
        {
            float elapsedTime = 0f;
            // Vòng lặp chạy cho đến khi hết thời gian tan biến
            while (elapsedTime < dissolveDuration)
            {
                elapsedTime += Time.deltaTime;
                // Tính toán giá trị dissolve dựa trên phần trăm thời gian đã trôi qua
                float dissolveAmount = Mathf.Clamp01(elapsedTime / dissolveDuration);

                foreach (var material in allMaterials)
                {
                    material.SetFloat(DISSOLVE_AMOUNT_PROPERTY, dissolveAmount);
                }

                yield return null;
            }
        }
    }

    public IEnumerator AppearCo(float dissolveDuration)
    {
        if (allMaterials.Count > 0)
        {
            float elapsedTime = 0f;
            while (elapsedTime < dissolveDuration)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = 1f - Mathf.Clamp01(elapsedTime / dissolveDuration);

                foreach (var material in allMaterials)
                {
                    material.SetFloat(DISSOLVE_AMOUNT_PROPERTY, dissolveAmount);
                }
                yield return null;
            }

            foreach (var material in allMaterials)
            {
                material.SetFloat(DISSOLVE_AMOUNT_PROPERTY, 0.08f);
            }
        }
    }
}

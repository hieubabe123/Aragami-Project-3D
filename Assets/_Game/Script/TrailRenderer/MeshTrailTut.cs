using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MeshTrailTut : MonoBehaviour
{
    [SerializeField] private float meshSpawnInterval = 0.05f;
    [SerializeField] private float meshFadeDuration = 0.8f;

    [Header("Shader Related")]
    [SerializeField] private Material trailMaterial;
    private const string FADE_PROPERTY = "_Alpha";
    [SerializeField] private string shaderVarRef;
    [SerializeField] private float shaderVarRate = 0.1f;
    [SerializeField] private float shaderVarRefreshRate = 0.05f;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;


    void Awake()
    {
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public IEnumerator PlayerTrail(Vector3 startPos, Vector3 endPoint, float travelDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < travelDuration)
        {
            Vector3 currentPos = Vector3.Lerp(startPos, endPoint, elapsedTime / travelDuration);

            SpawnMeshSnapshotAt(currentPos, transform.rotation);
            elapsedTime += meshSpawnInterval;
            yield return new WaitForSeconds(meshSpawnInterval);
        }
    }

    public void SpawnMeshSnapshotAt(Vector3 position, Quaternion rotation)
    {
        foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderers)
        {
            GameObject snapshotObj = new GameObject("Aragami_Shadow");
            snapshotObj.transform.SetPositionAndRotation(position, rotation);

            MeshFilter meshF = snapshotObj.AddComponent<MeshFilter>();
            MeshRenderer meshR = snapshotObj.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();
            renderer.BakeMesh(mesh);
            meshF.mesh = mesh;

            Material materialInstance = new Material(trailMaterial);

            int subMeshCount = mesh.subMeshCount;
            Material[] materials = new Material[subMeshCount];
            for (int i = 0; i < subMeshCount; i++)
            {
                materials[i] = materialInstance;
            }
            meshR.materials = materials;

            StartCoroutine(FadeAndDestroySnapshot(materialInstance, snapshotObj, meshFadeDuration));
        }
    }

    private IEnumerator FadeAndDestroySnapshot(Material material, GameObject snapshotObj, float fadeDuration)
    {
        float elapsedTime = 0f;
        material.SetFloat(FADE_PROPERTY, 1f);
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / fadeDuration);
            material.SetFloat(FADE_PROPERTY, alpha);
            yield return null;
        }

        Destroy(snapshotObj);
    }

}

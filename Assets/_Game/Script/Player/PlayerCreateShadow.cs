using UnityEngine;

public class PlayerCreateShadow : MonoBehaviour
{
    private ObjectPooling _shadowObjPrefab;
    void Awake()
    {
        _shadowObjPrefab = GetComponent<ObjectPooling>();
    }

    public void CreateShadowAt(Vector3 targetPosition)
    {
        GameObject shadowObj = _shadowObjPrefab.GetObject();

        if (shadowObj != null)
        {
            shadowObj.transform.position = targetPosition;
            shadowObj.transform.rotation = Quaternion.Euler(90, 0, 0);
            shadowObj.SetActive(true);
        }
    }
}

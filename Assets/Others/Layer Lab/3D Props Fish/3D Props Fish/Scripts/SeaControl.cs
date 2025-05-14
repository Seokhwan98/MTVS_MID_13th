using UnityEngine;

public class SeaControl : MonoBehaviour
{
    public static SeaControl Instance { get; private set; }

    [SerializeField] private Collider seaCollider;

    private Bounds _bounds;
    private bool _boundsInitialized = false;

    private void Awake()
    {
        Instance = this;
        CalculateBounds();
    }
    
    public void InitializeBounds()
    {
        _bounds = seaCollider.bounds;
        Debug.Log($"[SeaControl] Bounds 재계산: center={_bounds.center}, size={_bounds.size}");
    }

    private void CalculateBounds()
    {
        if (seaCollider == null)
        {
            Debug.LogError("[SeaControl] seaCollider가 비어 있습니다.");
            return;
        }

        _bounds = seaCollider.bounds;
        _boundsInitialized = true;

        Debug.Log($"[SeaControl] 콜라이더 바운드 계산 완료: center={_bounds.center}, size={_bounds.size}");
    }

    public Vector3 GetRandomPositionInSea()
    {
        if (!_boundsInitialized)
        {
            Debug.LogWarning("[SeaControl] Bounds가 초기화되지 않았습니다.");
            return transform.position;
        }

        Vector3 center = _bounds.center;
        float radiusX = _bounds.extents.x;
        float radiusZ = _bounds.extents.z;
        float height  = _bounds.size.y;

        for (int i = 0; i < 100; i++)
        {
            float localX = Random.Range(-radiusX, radiusX);
            float localZ = Random.Range(-radiusZ, radiusZ);
            float normX = localX / radiusX;
            float normZ = localZ / radiusZ;

            if (normX * normX + normZ * normZ <= 1f)
            {
                float localY = Random.Range(-height * 0.4f, height * 0.4f);
                Vector3 localOffset = new Vector3(localX, localY, localZ);
                return center + localOffset;
            }
        }

        return center;
    }

    public Bounds GetSeaBounds() => _bounds;
}

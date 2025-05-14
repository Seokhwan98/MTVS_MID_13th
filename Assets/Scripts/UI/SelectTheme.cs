using UnityEngine;

public class SelectTheme : MonoBehaviour
{
    [SerializeField] private GameObject hotPrefab;
    [SerializeField] private GameObject warmPrefab;
    [SerializeField] private ARObjectOnPlane onPlane;
    
    public void OnClickHot()
    {
        onPlane.spawnPrefab = hotPrefab;
    }

    public void OnClickWarm()
    {
        onPlane.spawnPrefab = warmPrefab;
    }
}

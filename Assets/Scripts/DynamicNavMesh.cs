using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : MonoBehaviour
{
    private NavMeshSurface _surface;  
    private bool isNavMeshBuilt = false;
    public event Action OnNavMeshBuilt;

    void Start()
    {
        _surface = GetComponent<NavMeshSurface>();
        BuildNavMesh();
    }

    public void BuildNavMesh()
    {
        _surface.BuildNavMesh();  // NavMesh 빌드
        isNavMeshBuilt = true;
        Debug.Log("NavMesh Built Successfully!");

        // 이벤트 호출
        OnNavMeshBuilt?.Invoke();
    }
    public bool IsNavMeshBuilt()
    {
        return isNavMeshBuilt;
    }
}

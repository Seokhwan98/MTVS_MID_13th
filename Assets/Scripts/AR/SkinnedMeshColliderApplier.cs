using UnityEngine;

public class SkinnedMeshColliderApplier : MonoBehaviour
{
    public GameObject colliderTarget;

    void Start()
    {
        var skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        var bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        if (colliderTarget == null)
        {
            colliderTarget = new GameObject("BakedCollider");
            colliderTarget.transform.SetParent(transform);
            colliderTarget.transform.localPosition = Vector3.zero;
            colliderTarget.transform.localRotation = Quaternion.identity;
            colliderTarget.layer = LayerMask.NameToLayer("Fish");
        }

        var meshCollider = colliderTarget.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
    }
}
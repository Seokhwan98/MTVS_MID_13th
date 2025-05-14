using System.Linq;
using UnityEngine;

public class FreeSwimming : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float minSpeed = 1f;
    public float rotationSpeed = 2f;
    public float avoidanceDistance = 5f;
    public LayerMask obstacleLayer;
    public float targetProximityThreshold = 1f;

    private Vector3 _targetPosition;
    private Collider[] _obstacles;

    private void Start()
    {
        // 월드 기준 위치 → 부모 기준 로컬 위치로 변환
        Vector3 world = SeaControl.Instance.GetRandomPositionInSea();
        transform.localPosition = transform.parent.InverseTransformPoint(world);

        // 타겟은 계속 월드 좌표로 사용
        SetRandomTargetPosition();
    }

    private void Update()
    {
        bool outOfBounds = !IsInsideCylinder(transform.position);

        if (Vector3.Distance(transform.position, _targetPosition) <= targetProximityThreshold
            || outOfBounds)
        {
            SetRandomTargetPosition();
        }

        
        MoveTowardsTarget();
        
    }

    private void SetRandomTargetPosition()
    {
        _targetPosition = SeaControl.Instance.GetRandomPositionInSea();
    }

    private bool IsInsideCylinder(Vector3 worldPos)
    {
        var bounds = SeaControl.Instance.GetSeaBounds();
        Vector3 center = bounds.center;

        // world 단위 반지름 & 높이
        float rx = bounds.extents.x;
        float rz = bounds.extents.z;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;

        Vector3 d = worldPos - center;
        bool insideHoriz =
            (d.x*d.x)/(rx*rx)
            + (d.z*d.z)/(rz*rz)
            <= 1f;
        bool insideVert = worldPos.y >= minY && worldPos.y <= maxY;

        return insideHoriz && insideVert;
    }



    private void MoveTowardsTarget()
    {
        _obstacles = Physics.OverlapSphere(transform.position, avoidanceDistance, obstacleLayer);

        if (_obstacles.Length > 0)
        {
            var avoidanceDirection = _obstacles.Aggregate(Vector3.zero,
                (current, obstacle) => current + (transform.position - obstacle.transform.position).normalized);
            var avoidanceRotation = Quaternion.LookRotation(avoidanceDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, avoidanceRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            var direction = _targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float distance = direction.magnitude;
            float scaleFactor = transform.lossyScale.magnitude;
            float speedMultiplier = Mathf.Clamp01(distance / avoidanceDistance);
            float speed = Mathf.Lerp(minSpeed, maxSpeed, speedMultiplier) * scaleFactor;

            // ✅ 이동하기 전에 나갈지 예측
            Vector3 move = transform.forward * (speed * Time.deltaTime);
            Vector3 nextPosition = transform.position + move;

            if (IsInsideCylinder(nextPosition))
            {
                transform.Translate(move, Space.World);
            }
            else
            {
                SetRandomTargetPosition();
            }
        }
    }
}

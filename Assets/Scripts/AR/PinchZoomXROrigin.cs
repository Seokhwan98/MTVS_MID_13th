using UnityEngine;
using Unity.XR.CoreUtils; // XROrigin 접근

public class PinchZoomXROrigin : MonoBehaviour
{
    private XROrigin xrOrigin;
    [SerializeField] private float zoomSpeed = 0.005f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxDistance = 4f;

    private float initialDistance;
    private Vector3 initialOriginPosition;

    void Awake()
    {
        xrOrigin = GetComponent<XROrigin>();
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            var t1 = Input.GetTouch(0);
            var t2 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(t1.position, t2.position);
                initialOriginPosition = xrOrigin.Origin.transform.position;
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(t1.position, t2.position);
                float delta = currentDistance - initialDistance;

                Vector3 camForward = xrOrigin.Camera.transform.forward;
                Vector3 offset = camForward * delta * zoomSpeed;
                Vector3 target = initialOriginPosition - offset;

                float dist = Vector3.Distance(target, xrOrigin.Camera.transform.position);
                if (dist >= minDistance && dist <= maxDistance)
                {
                    xrOrigin.Origin.transform.position = target;
                }
            }
        }
    }
}
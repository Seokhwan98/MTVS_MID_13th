using UnityEngine;
using UnityEngine.InputSystem;

public class TouchDetector : MonoBehaviour
{
    [SerializeField] private LayerMask fishLayerMask;
    private Camera arCamera;

    private void Start()
    {
        arCamera = Camera.main;
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPoint = Mouse.current.position.ReadValue();
            TryRaycastFromScreenPoint(screenPoint);
        }
#else
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 screenPoint = Touchscreen.current.primaryTouch.position.ReadValue();
            TryRaycastFromScreenPoint(screenPoint);
        }
#endif
    }

    private void TryRaycastFromScreenPoint(Vector2 screenPoint)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, 50f, fishLayerMask))
        {
            var handler = hit.collider.GetComponentInParent<FishTouchHandler>();
            if (handler != null)
            {
                handler.OnTouched();
            }
        }
    }
}
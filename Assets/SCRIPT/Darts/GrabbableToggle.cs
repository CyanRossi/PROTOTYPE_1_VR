using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbableToggle : MonoBehaviour
{
    public Dart dart;
    public Transform rightHandSocket;

    // Input System actions
    public InputActionReference triggerClick;
    public InputActionReference powerAxis;

    // Hand tracking (optional)
    public OVRHand rightHand;

    bool holding = false;

    void OnEnable()
    {
        triggerClick.action.performed += OnClick;
        triggerClick.action.Enable();
        if (powerAxis) powerAxis.action.Enable();
    }
    void OnDisable()
    {
        triggerClick.action.performed -= OnClick;
        triggerClick.action.Disable();
        if (powerAxis) powerAxis.action.Disable();
    }

    void OnClick(InputAction.CallbackContext _)
    {
        if (!holding)
        {
            dart.PickUp(rightHandSocket);
            holding = true;
        }
        else
        {
            float powerMul = 1f;
            if (powerAxis) powerMul = Mathf.Lerp(0.6f, 1.8f, Mathf.Clamp01(powerAxis.action.ReadValue<float>()));
            dart.Release(powerMul);
            holding = false;
            GameEvents.OnDartThrown?.Invoke(dart);
        }
    }

    void Update()
    {
        // Hand tracking alternative: pinch to hold, unpinch to release
        if (rightHand != null && rightHand.IsTracked)
        {
            bool pinch = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            if (pinch && !holding)
            {
                dart.PickUp(rightHandSocket);
                holding = true;
            }
            else if (!pinch && holding)
            {
                dart.Release(1f);
                holding = false;
                GameEvents.OnDartThrown?.Invoke(dart);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;

[RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
public class ScalableModel : MonoBehaviour
{
    [Header("Scaling Settings")]
    [Tooltip("Minimum scale (percentage of original)")]
    [Range(0.1f, 0.5f)] public float minScale = 0.3f;

    [Tooltip("Maximum scale (percentage of original)")]
    [Range(1.5f, 20f)] public float maxScale = 5f;

    [Tooltip("How fast scaling occurs")]
    [Range(0.1f, 10f)] public float scaleSpeed = 3f;

    [Header("Interaction Settings")]
    [Tooltip("Require grab to allow scaling")]
    public bool requireGrabToScale = false;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Vector3 initialScale;
    private HandsAggregatorSubsystem handsAggregator;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        initialScale = transform.localScale;

        // Basic Rigidbody setup for physics + smooth grab
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        grabInteractable.selectEntered.AddListener(_ => OnGrabbed());

        handsAggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
    }

    private void Update()
    {
        if (requireGrabToScale && !grabInteractable.isSelected)
            return;

        float scaleInput = 0f;

        // Keyboard testing
        if (Keyboard.current != null) {
            if (Keyboard.current.cKey.isPressed) scaleInput = 1f;
            else if (Keyboard.current.vKey.isPressed) scaleInput = -1f;
        }

        // Hand tracking
        if (handsAggregator != null &&
            handsAggregator.TryGetPinchProgress(XRNode.RightHand, out _, out bool isPinching, out float pinchAmount)) {
            if (isPinching)
                scaleInput = pinchAmount;
        }

        if (scaleInput != 0f) {
            ScaleModel(scaleInput * scaleSpeed * Time.deltaTime);
        }
    }

    private void OnGrabbed()
    {
        handsAggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
    }

    private void ScaleModel(float scaleDelta)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleDelta;

        newScale.x = Mathf.Clamp(newScale.x, initialScale.x * minScale, initialScale.x * maxScale);
        newScale.y = Mathf.Clamp(newScale.y, initialScale.y * minScale, initialScale.y * maxScale);
        newScale.z = Mathf.Clamp(newScale.z, initialScale.z * minScale, initialScale.z * maxScale);

        transform.localScale = newScale;
    }
}

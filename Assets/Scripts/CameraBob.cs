using TMPro;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public float bobDistance = 0.1f;
    public float bobSpeed = 1f;

    public float rotationAmount = 5f;   // how far it tilts
    public float rotationSpeed = 1f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public Transform yesPoint;
    public Transform noPoint;
    public Transform finalNoPoint;

    

    public float moveSmoothTime = 0.2f;
    private Vector3 velocity;

    public enum CameraState
    {
        Idle,
        YesView,
        NoView
    }

    private CameraState state = CameraState.Idle;
    void Start()
    {
        targetPosition = transform.localPosition;
        targetRotation = transform.localRotation;
    }

    void IdleBob()
    {

        float zRot = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
        transform.localRotation = targetRotation * Quaternion.Euler(zRot, zRot, 0f);

        transform.localPosition = targetPosition;
    }

    void YesBob()
    {
        transform.position = Vector3.SmoothDamp(
        transform.position,
        targetPosition,
        ref velocity,
        moveSmoothTime
    );

        float zRot = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
        transform.rotation = targetRotation * Quaternion.Euler(zRot, zRot, 0f);
    }

    void NoBob()
    {
        float zOffset = Mathf.Sin(Time.time * bobSpeed) * bobDistance;
        transform.position = targetPosition + Vector3.forward * zOffset;

        transform.rotation = targetRotation;
    }

    public void SetYesView()
    {
        state = CameraState.YesView;
        ApplyTarget(yesPoint);
    }

    public void SetNoView()
    {
        state = CameraState.NoView;
        ApplyTarget(noPoint);
    }

   

    public void SetIdle()
    {
        state = CameraState.Idle;
    }

    void ApplyTarget(Transform point)
    {
        if (point == null) return;

        targetPosition = point.position;
        targetRotation = point.rotation;
    }
    void Update()
    {
        switch (state)
        {
            case CameraState.Idle:
                IdleBob();
                break;

            case CameraState.YesView:
                YesBob();
                break;

            case CameraState.NoView:
                NoBob();
                break;
        }
    }
}
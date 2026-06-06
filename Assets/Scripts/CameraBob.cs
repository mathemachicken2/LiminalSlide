using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public float bobDistance = 0.1f;
    public float bobSpeed = 1f;

    public float rotationAmount = 5f;   // how far it tilts
    public float rotationSpeed = 1f;

    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    void Update()
    {
       
        float zRot = 0f;
        zRot = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
        transform.localRotation = startLocalRotation * Quaternion.Euler(zRot, zRot, 0f);


        if (Input.GetKey(KeyCode.Space))
        {
            float zOffset = Mathf.Sin(Time.time * bobSpeed) * bobDistance;
            transform.localPosition = startLocalPosition + Vector3.forward * zOffset;
        }
        else
        {
            transform.localPosition = startLocalPosition;
        }
    }
}
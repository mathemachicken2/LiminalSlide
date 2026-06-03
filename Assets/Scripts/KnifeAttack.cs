using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 8f;
    public float rotationSpeed = 300f;

    void Update()
    {
        if (target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        transform.Rotate(
            0f,
            0f,
            rotationSpeed * Time.deltaTime
        );

       
    }
}
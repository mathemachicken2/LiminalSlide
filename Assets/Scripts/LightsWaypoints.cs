using UnityEngine;

public class SlideLoop : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private float t = 0f;

    void Update()
    {
        t += Time.deltaTime * speed;

        // move from A to B
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);

        // reset when reaching end
        if (t >= 1f)
        {
            t = 0f;
        }
    }
}
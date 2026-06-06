using UnityEngine;

public class SlideWinMovement : MonoBehaviour
{
    public Transform winEndPoint;
    public float moveSpeed = 3f;

    private bool movingToWin = false;

    public void StartWinMovement()
    {
        movingToWin = true;
    }

    void Update()
    {
        if (!movingToWin || winEndPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            winEndPoint.position,
            moveSpeed * Time.deltaTime
        );
    }
}
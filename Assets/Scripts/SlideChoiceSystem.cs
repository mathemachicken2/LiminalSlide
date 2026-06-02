using UnityEngine;

public class SlideChoiceSystem : MonoBehaviour
{
    [Header("Moving Object")]
    public Transform movingObject;

    [Header("Waypoints")]
    public Transform startPoint;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Movement")]
    public float speed = 2f;

    private Transform currentTarget;

    [Header("Timing")]
    public float minChoiceTime = 7f;
    public float maxChoiceTime = 14f;

    private float timer = 0f;
    private float choiceTimer = 0f;
    private bool waitingForChoice = false;

    [Header("UI")]
    public GameObject choicePanel;

    void Start()
    {
       
        currentTarget = startPoint;

        movingObject.position = startPoint.position;

        choiceTimer = Random.Range(minChoiceTime, maxChoiceTime);
        choicePanel.SetActive(false);
    }

    void Update()
    {
        if (waitingForChoice) return;

        // move object
        movingObject.position = Vector3.MoveTowards(
            movingObject.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        timer += Time.deltaTime;

        // trigger choice
        if (timer >= choiceTimer)
        {
            StartChoice();
        }
    }

    void StartChoice()
    {
        waitingForChoice = true;
        choicePanel.SetActive(true);
    }

    void ResumeMovement(Transform newTarget)
    {
        currentTarget = newTarget;

        timer = 0f;
        choiceTimer = Random.Range(minChoiceTime, maxChoiceTime);

        waitingForChoice = false;
        choicePanel.SetActive(false);
    }

    // UI BUTTONS
    public void ChooseLeft()
    {
        ResumeMovement(leftPoint);
    }

    public void ChooseRight()
    {
        ResumeMovement(rightPoint);
    }
}
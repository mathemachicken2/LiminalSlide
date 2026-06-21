using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlideLoop : MonoBehaviour
{
    [Header("Movement")]
    public Transform movingObject;
    public float speed = 2f;

    [Header("Points")]
    public Transform pointA;
    public Transform pointB;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("UI")]
    public GameObject choicePanel;

    private Transform currentEnd;
    private float t;

    private bool waitingForChoice = false;
    private bool inBranch = false;

    private float loopTimer;
    private float nextChoiceTime;

    [Header("Death")]
    public DeathSequence deathSequence;

    public AudioClip choiceSelectedSound;

    void Start()
    {
        movingObject.position = pointA.position;
        SetNextChoiceTime();
        currentEnd = pointB; // normal loop A → B
        choicePanel.SetActive(false);
    }

    void Update()
    {
        if (movingObject == null || currentEnd == null) return;
        if (waitingForChoice) return;

        t += Time.deltaTime * speed;
        loopTimer += Time.deltaTime;

        if (t > 1f)
        {
            t -= 1f;

            // Reached end of segment
            if (!inBranch && currentEnd == pointB && loopTimer >= nextChoiceTime)
            {
                ShowChoices();
                return;
            }
        }

        movingObject.position = Vector3.Lerp(
            pointA.position,
            currentEnd.position,
            t
        );
    }

    void SetNextChoiceTime()
    {
        nextChoiceTime = Random.Range(3f, 4f);
        loopTimer = 0f;
    }

    public void ShowChoices()
    {
        waitingForChoice = true;
        choicePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ChooseLeft()
    {
        StartBranch(leftPoint);
        StartCoroutine(ClearSelectionNextFrame());
    }

    public void ChooseRight()
    {
        StartBranch(rightPoint);
        StartCoroutine(ClearSelectionNextFrame());
    }

    IEnumerator ClearSelectionNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void StartBranch(Transform branchPoint)
    {
        GameAudio.Instance.Play(choiceSelectedSound);

        if (deathSequence != null)
        {
            deathSequence.CheckForDeath();
        }

        Time.timeScale = 1f;
        choicePanel.SetActive(false);

        currentEnd = branchPoint;
        t = 0f;

        waitingForChoice = false;
        inBranch = true;

        StartCoroutine(ReturnToMainLoopAfterDelay());
    }

    IEnumerator ReturnToMainLoopAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        if (deathSequence != null && deathSequence.isDead)
            yield break;

        // Return to main loop
        currentEnd = pointB;
        t = 0f;
        inBranch = false;

        SetNextChoiceTime(); 
    }
}
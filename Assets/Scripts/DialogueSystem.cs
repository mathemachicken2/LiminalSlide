using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Image fadeOverlay;
    public Image bloodOverlay;
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextLineIndex;

        public ChoiceAction action;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;

        public bool hasChoices;
        public DialogueChoice[] choices;
    }

    public GameObject dialoguePanel;

    public TMP_Text dialogueText;
    public TMP_Text speakerText;

    public DialogueLine[] dialogue;

    public float typingSpeed = 0.05f;

    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TMP_Text[] choiceButtonTexts;

    public CameraBob cameraBob;

    private DialogueChoice[] noSubChoices;
    private bool waitingForSubChoice = false;



    public enum ChoiceAction
    {
        None,
        Yes,
        No,
        NoBranch,
        GoBackInSlide
    }

    void Start()
    {
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        bloodOverlay.gameObject.SetActive(false);

        FadeOutAndStartDialogue(1.5f);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogue[index].line;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void FadeOutAndStartDialogue(float duration)
    {
        StartCoroutine(FadeOutRoutine(duration));
    }
    IEnumerator FadeOutRoutine(float duration)
    {
        Color c = fadeOverlay.color;
        float startAlpha = c.a;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            c.a = Mathf.Lerp(startAlpha, 0f, t);

            fadeOverlay.color = c;

            yield return null;
        }

        c.a = 0f;
        fadeOverlay.color = c;

        fadeOverlay.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        StartDialogue();
    }
    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        index = 0;
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (index >= dialogue.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine current = dialogue[index];

        speakerText.text = current.speaker;

        typingCoroutine = StartCoroutine(TypeLine(current.line));

        index++;

        // If this line has choices, we stop here AFTER typing
        if (current.hasChoices)
        {
            StartCoroutine(WaitThenShowChoices(current));
        }
    }

    IEnumerator WaitThenShowChoices(DialogueLine line)
    {
        while (isTyping)
            yield return null;

        ShowChoices(line.choices);
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtonTexts[i].text = choices[i].choiceText;

                DialogueChoice choice = choices[i];

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    OnChoiceSelected(choice);
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator FadeAndExecute(System.Action action, float duration)
    {
        fadeOverlay.gameObject.SetActive(true);
        action?.Invoke();
        yield return new WaitForSeconds(6f);
        

        Color c = fadeOverlay.color;
        float startAlpha = c.a;
        float time = 0f;

        

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            c.a = Mathf.Lerp(startAlpha, 1f, t);
            fadeOverlay.color = c;

            yield return null;
        }
        yield return new WaitForSeconds(1f);
        c.a = 1f;
        fadeOverlay.color = c;

        




    }
    void OnChoiceSelected(DialogueChoice choice)
    {
        Debug.Log("OnChoiceSelected entered");
        Debug.Log($"Action value: {choice.action}");

        choicePanel.SetActive(false);

        switch (choice.action)
        {
            case ChoiceAction.Yes:
                StartCoroutine(FadeAndExecute(YesAction, 2f));
                break;

            case ChoiceAction.NoBranch:
                StartCoroutine(FadeAndExecute(NoActionBranch, 2f));
                break;

            case ChoiceAction.No:
                Debug.Log("Final NO outcome");
                StartCoroutine(FadeAndExecute(NoAction, 2f));
                ShowDialogueText("Final NO response...");
                break;

            case ChoiceAction.GoBackInSlide:
                StartCoroutine(FadeAndExecute(LoadSceneAction, 2f));
                break;

            default:
                Debug.Log("NONE selected");
                break;
        }

        // ONLY continue dialogue if NOT in sub-choice mode
        if (!waitingForSubChoice)
        {
            index = choice.nextLineIndex;
            ShowNextLine();
        }
    }

    void LoadSceneAction()
    {
        Debug.Log("GO BACK TO SCENE");

       // SceneManager.LoadScene("SampleScene");
    }
    void YesAction()
    {
        Debug.Log("YES ACTION");

        cameraBob.SetYesView();

        ShowDialogueText("This is the YES path dialogue...");
    }

    void NoAction()
    {
        Debug.Log("NO ACTION");

        cameraBob.SetYesView();
        StartCoroutine(FadeInBlood(1f));
        ShowDialogueText("This is the NO path dialogue...");
    }
    void NoActionBranch()
    {
        Debug.Log("NO ACTION");

        cameraBob.SetNoView();

       

        noSubChoices = new DialogueChoice[]
        {
        new DialogueChoice
        {
            choiceText = "Okay",
            action = ChoiceAction.Yes
        },
        new DialogueChoice
        {
            choiceText = "No",
            action = ChoiceAction.No
        },
        new DialogueChoice
        {
            choiceText = "Hell no",
            action = ChoiceAction.No
        }
        };


        waitingForSubChoice = true;
        ShowChoices(noSubChoices);

        ShowDialogueText("This is the NO path dialogue...");
    }

    void ShowDialogueText(string text)
    {
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        dialogueText.text = text;
    }
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        speakerText.text = "";
    }
    IEnumerator FadeInBlood(float duration)
    {
        bloodOverlay.gameObject.SetActive(true);

        Color c = bloodOverlay.color;
        c.a = 0f;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = t / duration;

            c.a = alpha;
            bloodOverlay.color = c;

            yield return null;
        }

        c.a = 1f;
        bloodOverlay.color = c;
    }
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Image fadeOverlay;
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextLineIndex;
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

    void Start()
    {
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);

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

        ShowChoices(line);
    }

    void ShowChoices(DialogueLine line)
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < line.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtonTexts[i].text = line.choices[i].choiceText;

                int choiceIndex = line.choices[i].nextLineIndex;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    OnChoiceSelected(choiceIndex);
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(int nextIndex)
    {
        choicePanel.SetActive(false);
        index = nextIndex;
        ShowNextLine();
    }
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        speakerText.text = "";
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
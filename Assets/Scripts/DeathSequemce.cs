using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DeathSequence : MonoBehaviour
{
    [Header("Death Settings")]
    [Range(0f, 1f)]
    public float deathChance = 0.3f;

    public int pressesBeforeDeathCanHappen = 3;

    [Header("UI")]
    public GameObject bloodOverlay;
    private Image bloodOverlayImage;
    public GameObject gameOverPanel;

    private int buttonPressCount = 0;
    public bool isDead = false;

    public GameObject knifeRingPrefab;
    public Transform knifeSpawnPoint;
    public Transform knifeEndPoint;

    public GameObject bloodParticlePrefab;
    public Transform vfxSpawnPoint;
    public Transform secondVfxSpawnPoint;
    public Transform thirdVfxSpawnPoint;

    public GameObject winOverlay;
    public float winFadeDuration = 4f;
    public string winSceneName = "AfterLife";

    private bool hasWon = false;

    public SlideWinMovement slideWinMovement;

    private static int winCount = 0;
    public TMP_Text toBeContinuedText;

    //public AudioClip deathSound;
    public AudioClip knifeSound;
    public AudioClip bloodSound;
    public AudioClip winSound;

    private static bool hasBeenToAfterlife = false;
    private static bool endingTriggered = false;
    


    void Start()
    {
        toBeContinuedText.gameObject.SetActive(false);
        
        endingTriggered = false;
        winCount= 0;


        if (bloodOverlay != null)
        {
            bloodOverlayImage = bloodOverlay.GetComponent<Image>();
            bloodOverlay.SetActive(true);

            if (bloodOverlayImage != null)
            {
                Color c = bloodOverlayImage.color;
                c.a = 0f;
                bloodOverlayImage.color = c;
            }
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winOverlay != null)
        {
            winOverlay.SetActive(true);

            Image img = winOverlay.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }
    }

    public static void ResetProgress()
    {
        hasBeenToAfterlife = false;
        endingTriggered = false;
        winCount = 0;
    }
    IEnumerator FadeBloodOverlay(float duration)
    {
        if (bloodOverlayImage == null)
            yield break;

        float t = 0f;

        Color c = bloodOverlayImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(0f, 0.5f, t / duration);
            bloodOverlayImage.color = c;

            yield return null;
        }

        c.a = 0.5f;
        bloodOverlayImage.color = c;
    }
    IEnumerator ShowGameOverAfterDelay()
    {

        yield return new WaitForSeconds(4f);

        SpawnBloodVFX();

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(FadeBloodOverlay(4f));

        yield return new WaitForSeconds(2f);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
       
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f; // in case you pause later
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // make sure scene name exists in Build Settings
    }
    void SpawnBloodVFX()
    {
        StartCoroutine(BloodBurstRoutine());
    }

    IEnumerator BloodBurstRoutine()
    {
        GameAudio.Instance.Play(bloodSound);
        
        Instantiate(
            bloodParticlePrefab,
            vfxSpawnPoint.position,
            vfxSpawnPoint.rotation
        );

        yield return new WaitForSeconds(1f);

        // Second burst at another position
        if (secondVfxSpawnPoint != null)
        {
            Instantiate(
                bloodParticlePrefab,
                secondVfxSpawnPoint.position,
                secondVfxSpawnPoint.rotation
            );
        }
        if (thirdVfxSpawnPoint != null)
        {
            Instantiate(
                bloodParticlePrefab,
                thirdVfxSpawnPoint.position,
                thirdVfxSpawnPoint.rotation
            );
        }
    }

    public bool CheckForDeath()
    {
        if (isDead || hasWon)
            return true;

        buttonPressCount++;

        if (buttonPressCount <= pressesBeforeDeathCanHappen)
            return false;

        if (Random.value <= deathChance)
        {
            Die();
            return true;
        }

        if (buttonPressCount >= 4)
        {
            WinGame();
            return false;
        }

        return false;
    }

    void WinGame()
    {
        if (isDead || hasWon)
            return;

        hasWon = true;
        winCount++;

      

        GameAudio.Instance.Play(winSound);
        StartCoroutine(WinRoutine());
    }

    IEnumerator WinRoutine()
    {
        slideWinMovement.StartWinMovement();
        yield return new WaitForSeconds(2.5f);
        if (winOverlay != null)
        {
           

            Image img = winOverlay.GetComponent<Image>();

            float t = 0f;
            Color c = img.color;

            while (t < winFadeDuration)
            {
                t += Time.deltaTime;

                c.a = Mathf.Lerp(0f, 1f, t / winFadeDuration);
                img.color = c;

                yield return null;
            }

            c.a = 1f;
            img.color = c;
        }

        //yield return new WaitForSeconds(2f);
        if (endingTriggered)
            yield break;

        if (!hasBeenToAfterlife)
        {
            Debug.Log("First win → AfterLife");

            hasBeenToAfterlife = true;

            yield return new WaitForSeconds(0.5f);

            SceneManager.LoadScene(winSceneName);
        }
        else
        {
            endingTriggered = true;

            Debug.Log("Second progression win → Main Menu ending");

            toBeContinuedText.gameObject.SetActive(true);

            StartCoroutine(ShakeText(toBeContinuedText.rectTransform, 6f, 2f));

            yield return new WaitForSeconds(1f);

            SceneManager.LoadScene("MainMenu");
        }
        }
    IEnumerator ShakeTextDelay()
    {
        yield return new WaitForSeconds(2f);
        toBeContinuedText.gameObject.SetActive(true);
        StartCoroutine(ShakeText(toBeContinuedText.rectTransform, 6f, 2f));
    }
    IEnumerator ShakeText(RectTransform rect, float duration, float magnitude)
    {
        Vector3 originalPos = rect.anchoredPosition;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            rect.anchoredPosition = originalPos + new Vector3(x, y, 0f);

            yield return null;
        }

        rect.anchoredPosition = originalPos;
    }
   
    void Die()
    {
        isDead = true;
        Debug.Log($"Player died. Win count = {winCount}");
        StartCoroutine(ShowGameOverAfterDelay());

        SpawnKnifeRing();

        Debug.Log("Player died!");

       

    }
    void SpawnKnifeRing()
    {
        GameAudio.Instance.Play(knifeSound);
        GameObject knifeRing = Instantiate(
            knifeRingPrefab,
            knifeSpawnPoint.position,
            knifeSpawnPoint.rotation
        );

        KnifeAttack attack = knifeRing.GetComponent<KnifeAttack>();

        if (attack != null)
        {
            attack.target = knifeEndPoint;
        }
    }
}
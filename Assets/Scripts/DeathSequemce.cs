using System.Collections;
using UnityEngine;
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


    void Start()
    {
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

    void SpawnBloodVFX()
    {
        StartCoroutine(BloodBurstRoutine());
    }

    IEnumerator BloodBurstRoutine()
    {
        // First burst
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
        if (isDead)
            return true;

        buttonPressCount++;

        // Only start rolling after 3 presses
        if (buttonPressCount <= pressesBeforeDeathCanHappen)
            return false;

        if (Random.value <= deathChance)
        {
            Die();
            return true;
        }

        return false;
    }

    void Die()
    {
        isDead = true;

      

        StartCoroutine(ShowGameOverAfterDelay());

        SpawnKnifeRing();

        Debug.Log("Player died!");

        // Optional:
        // Time.timeScale = 0f;
        // Show death screen
        // Play sound
        // Disable controls
    }
    void SpawnKnifeRing()
    {
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
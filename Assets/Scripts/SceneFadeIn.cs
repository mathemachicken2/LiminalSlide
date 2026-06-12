using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    void Start()
    {
       
        fadeImage.gameObject.SetActive(true);

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        StartCoroutine(FadeFromWhite());
    }

    IEnumerator FadeFromWhite()
    {
        float t = 0f;

        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;

        fadeImage.gameObject.SetActive(false);
    }
}
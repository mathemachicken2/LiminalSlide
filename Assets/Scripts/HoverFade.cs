using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;

    [Range(0f, 1f)]
    public float normalAlpha = 1f;
    public float hoverAlpha = 0.5f;

    public float fadeSpeed = 10f;

    private float targetAlpha;
    private Color currentColor;

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        currentColor = targetImage.color;
        targetAlpha = normalAlpha;
    }

    void Update()
    {
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        targetImage.color = currentColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetAlpha = hoverAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetAlpha = normalAlpha;
    }
}
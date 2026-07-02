using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public static FadeEffect Instance;

    public Image fadePanel;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
        fadePanel.raycastTarget = false;
        fadePanel.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, elapsed / fadeDuration);
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 1);
    }

    public IEnumerator FadeIn()
    {
        fadePanel.color = new Color(0, 0, 0, 1);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, 1 - (elapsed / fadeDuration));
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 0);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Vereist voor TextMeshPro
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI statusText; // Sleep hier je UI tekst naartoe
    public float fadeSpeed = 1.0f;

    void Start()
    {
        // Start de game met een fade-in
        if (fadeImage != null) StartCoroutine(FadeIn());
    }

    // Wordt aangeroepen door AnomalyManager
    public void SetStatusText(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
        }
    }

    public IEnumerator FadeOut()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
    }

    // Deze methode zorgt dat de achtergrond en de tekst PRECIES tegelijk faden
    private void SetAlpha(float alpha)
    {
        float clampedAlpha = Mathf.Clamp01(alpha);

        // Update zwart vlak
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = clampedAlpha;
            fadeImage.color = c;
        }
        
        // Update tekst (fadet mee op exact hetzelfde tempo)
        if (statusText != null)
        {
            Color tc = statusText.color;
            tc.a = clampedAlpha;
            statusText.color = tc;
        }
    }
}